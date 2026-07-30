import { useCallback, useEffect, useState } from "react";
import { DataTable } from "primereact/datatable";
import { Column } from "primereact/column";
import { InputText } from "primereact/inputtext";
import { InputTextarea } from "primereact/inputtextarea";
import { Dropdown } from "primereact/dropdown";
import { Calendar } from "primereact/calendar";
import { Button } from "primereact/button";
import { ConfirmDialog, confirmDialog } from "primereact/confirmdialog";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faPlus, faTrash } from "@fortawesome/free-solid-svg-icons";

import SubResourceList, { type SubResourceColumn } from "../../components/SubResourceList";
import {
  deleteApiV1ApiresourcesByApiResourceIdClaimsById,
  deleteApiV1ApiresourcesByApiResourceIdPropertiesById,
  deleteApiV1ApiresourcesByApiResourceIdScopesById,
  deleteApiV1ApiresourcesByApiResourceIdSecretsById,
  getApiV1ApiresourcesByApiResourceIdClaims,
  getApiV1ApiresourcesByApiResourceIdProperties,
  getApiV1ApiresourcesByApiResourceIdScopes,
  getApiV1ApiresourcesByApiResourceIdSecrets,
  getApiV1Scopes,
  postApiV1ApiresourcesByApiResourceIdClaims,
  postApiV1ApiresourcesByApiResourceIdProperties,
  postApiV1ApiresourcesByApiResourceIdScopes,
  postApiV1ApiresourcesByApiResourceIdSecrets,
} from "../../api/generated/sdk.gen";
import type {
  ApiResourceClaimDto,
  ApiResourcePropertyDto,
  ApiResourceScopeDto,
  ApiResourceSecretDto,
  SaveApiResourceSecretDto,
} from "../../api/generated/types.gen";
import { SECRET_TYPES, validateSecretValue } from "../../utils/secretTypes";
import { CLAIM_TYPES } from "../../utils/knownValues";
import { problemMessage } from "../../utils/apiError";

interface PanelProps {
  apiResourceId: number;
}

function SingleValuePanel<T extends { id?: number | string }>({
  title,
  emptyMessage,
  field,
  header,
  placeholder,
  options,
  load,
  create,
  remove,
  describe,
}: Readonly<{
  title: string;
  emptyMessage: string;
  field: keyof T & string;
  header: string;
  placeholder?: string;
  /** When provided, renders an editable dropdown of known values instead of a plain text input. */
  options?: string[];
  load: () => Promise<T[]>;
  create: (value: string) => Promise<void>;
  remove: (id: number) => Promise<void>;
  describe: (row: T) => string;
}>) {
  const columns: SubResourceColumn<T>[] = [{ field, header }];

  return (
    <SubResourceList<T, { value: string }>
      title={title}
      emptyMessage={emptyMessage}
      load={load}
      remove={remove}
      describe={describe}
      columns={columns}
      emptyCreateDraft={{ value: "" }}
      renderCreateForm={({ onCreated, createDraft, setCreateDraft }) => {
        const submit = async () => {
          if (!createDraft.value.trim()) return;
          await create(createDraft.value.trim());
          onCreated();
        };
        return (
          <div className="flex gap-2">
            {options ? (
              <Dropdown
                editable
                className="flex-1"
                value={createDraft.value}
                options={options}
                placeholder={placeholder}
                onChange={(e) => setCreateDraft({ value: e.value ?? "" })}
              />
            ) : (
              <InputText
                className="flex-1"
                value={createDraft.value}
                placeholder={placeholder}
                onChange={(e) => setCreateDraft({ value: e.target.value })}
                onKeyDown={(e) => e.key === "Enter" && submit()}
              />
            )}
            <Button
              type="button"
              onClick={submit}
              label="Add"
              icon={<FontAwesomeIcon icon={faPlus} />}
              disabled={!createDraft.value.trim()}
            />
          </div>
        );
      }}
    />
  );
}

export function ClaimsPanel({ apiResourceId }: Readonly<PanelProps>) {
  return (
    <SingleValuePanel<ApiResourceClaimDto>
      title="User claims"
      emptyMessage="No user claims defined."
      field="type"
      header="Type"
      placeholder="email"
      options={CLAIM_TYPES}
      load={async () => {
        const r = await getApiV1ApiresourcesByApiResourceIdClaims({ path: { apiResourceId } });
        return r.error ? [] : (r.data ?? []);
      }}
      create={async (type) => {
        await postApiV1ApiresourcesByApiResourceIdClaims({
          path: { apiResourceId },
          body: { type },
        });
      }}
      remove={async (id) => {
        await deleteApiV1ApiresourcesByApiResourceIdClaimsById({ path: { apiResourceId, id } });
      }}
      describe={(row) => row.type ?? ""}
    />
  );
}

export function ScopesPanel({ apiResourceId }: Readonly<PanelProps>) {
  const [assigned, setAssigned] = useState<ApiResourceScopeDto[]>([]);
  const [allScopeNames, setAllScopeNames] = useState<string[]>([]);
  const [selected, setSelected] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);
  const [adding, setAdding] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const refresh = useCallback(async () => {
    setLoading(true);
    const [assignedResult, apiScopesResult] = await Promise.all([
      getApiV1ApiresourcesByApiResourceIdScopes({ path: { apiResourceId } }),
      getApiV1Scopes(),
    ]);
    if (!assignedResult.error) setAssigned(assignedResult.data ?? []);
    const names = new Set<string>();
    for (const s of apiScopesResult.data ?? []) if (s.name) names.add(s.name);
    setAllScopeNames([...names].sort((a, b) => a.localeCompare(b)));
    setLoading(false);
  }, [apiResourceId]);

  useEffect(() => {
    void refresh();
  }, [refresh]);

  const assignedNames = new Set(assigned.map((s) => s.scope));
  const available = allScopeNames
    .filter((name) => !assignedNames.has(name))
    .map((name) => ({ label: name, value: name }));

  const handleDelete = (row: ApiResourceScopeDto) => {
    confirmDialog({
      message: (
        <span>
          Remove scope <strong>{row.scope}</strong>?
        </span>
      ),
      header: "Confirm removal",
      acceptClassName: "p-button-danger",
      acceptLabel: "Remove",
      rejectLabel: "Cancel",
      accept: async () => {
        await deleteApiV1ApiresourcesByApiResourceIdScopesById({ path: { apiResourceId, id: Number(row.id) } });
        await refresh();
      },
    });
  };

  const handleAdd = async () => {
    if (!selected) return;
    setAdding(true);
    setError(null);
    try {
      const r = await postApiV1ApiresourcesByApiResourceIdScopes({ path: { apiResourceId }, body: { scope: selected } });
      if (r.error) {
        setError(problemMessage(r.error, "Failed to add scope."));
        return;
      }
      setSelected(null);
      await refresh();
    } finally {
      setAdding(false);
    }
  };

  return (
    <div className="overflow-hidden rounded-xl border border-border bg-surface shadow-sm">
      <ConfirmDialog />

      <header className="flex items-baseline justify-between gap-3 border-b border-border px-5 py-3.5">
        <div>
          <h3 className="text-sm font-semibold text-content">Scopes</h3>
          <p className="mt-0.5 text-xs text-content-muted">
            {loading ? "Loading…" : `${assigned.length} ${assigned.length === 1 ? "entry" : "entries"}`}
          </p>
        </div>
      </header>

      <DataTable value={assigned} loading={loading} dataKey="id" emptyMessage="No scopes associated with this API." size="small">
        <Column field="scope" header="Scope" />
        <Column
          header=""
          style={{ width: "4.5rem" }}
          body={(row: ApiResourceScopeDto) => (
            <div className="flex justify-end">
              <Button
                text
                rounded
                size="small"
                severity="danger"
                aria-label="Delete"
                icon={<FontAwesomeIcon icon={faTrash} />}
                onClick={() => handleDelete(row)}
              />
            </div>
          )}
        />
      </DataTable>

      <div className="border-t border-border bg-surface-muted px-5 py-4">
        <h4 className="mb-3 text-xs font-semibold uppercase tracking-wider text-content-muted">
          Add new
        </h4>
        <div className="flex gap-2">
          <Dropdown
            value={selected}
            options={available}
            onChange={(e) => setSelected(e.value as string)}
            placeholder={available.length === 0 ? "All scopes already added" : "Select a scope"}
            disabled={available.length === 0}
            filter
            className="flex-1"
          />
          <Button
            type="button"
            onClick={handleAdd}
            label="Add"
            icon={<FontAwesomeIcon icon={faPlus} />}
            disabled={!selected || adding}
            loading={adding}
          />
        </div>
        {error && <p className="mt-2 text-sm text-red-600">{error}</p>}
      </div>
    </div>
  );
}

export function PropertiesPanel({ apiResourceId }: PanelProps) {
  return (
    <SubResourceList<ApiResourcePropertyDto, { key: string; value: string }>
      title="Custom properties"
      emptyMessage="No custom properties."
      load={async () => {
        const r = await getApiV1ApiresourcesByApiResourceIdProperties({ path: { apiResourceId } });
        return r.error ? [] : (r.data ?? []);
      }}
      remove={async (id) => {
        await deleteApiV1ApiresourcesByApiResourceIdPropertiesById({ path: { apiResourceId, id } });
      }}
      describe={(row) => `${row.key ?? ""} = ${row.value ?? ""}`}
      columns={[
        { field: "key", header: "Key" },
        { field: "value", header: "Value" },
      ]}
      emptyCreateDraft={{ key: "", value: "" }}
      renderCreateForm={({ onCreated, createDraft, setCreateDraft }) => {
        const submit = async () => {
          if (!createDraft.key.trim() || !createDraft.value.trim()) return;
          await postApiV1ApiresourcesByApiResourceIdProperties({
            path: { apiResourceId },
            body: { key: createDraft.key.trim(), value: createDraft.value.trim() },
          });
          onCreated();
        };
        return (
          <div className="flex gap-2">
            <InputText
              className="flex-1"
              value={createDraft.key}
              placeholder="key"
              onChange={(e) => setCreateDraft({ ...createDraft, key: e.target.value })}
              onKeyDown={(e) => e.key === "Enter" && submit()}
            />
            <InputText
              className="flex-1"
              value={createDraft.value}
              placeholder="value"
              onChange={(e) => setCreateDraft({ ...createDraft, value: e.target.value })}
              onKeyDown={(e) => e.key === "Enter" && submit()}
            />
            <Button
              type="button"
              onClick={submit}
              label="Add"
              icon={<FontAwesomeIcon icon={faPlus} />}
              disabled={!createDraft.key.trim() || !createDraft.value.trim()}
            />
          </div>
        );
      }}
    />
  );
}

export function SecretsPanel({ apiResourceId }: PanelProps) {
  const emptyDraft: SaveApiResourceSecretDto = { type: "SharedSecret", value: "" };
  const [draft, setDraft] = useState<SaveApiResourceSecretDto>(emptyDraft);
  const [expiresAt, setExpiresAt] = useState<Date | null>(null);
  const [error, setError] = useState<string | null>(null);

  return (
    <SubResourceList<ApiResourceSecretDto, SaveApiResourceSecretDto>
      title="API resource secrets"
      emptyMessage="No secrets configured."
      load={async () => {
        const r = await getApiV1ApiresourcesByApiResourceIdSecrets({ path: { apiResourceId } });
        return r.error ? [] : (r.data ?? []);
      }}
      remove={async (id) => {
        await deleteApiV1ApiresourcesByApiResourceIdSecretsById({ path: { apiResourceId, id } });
      }}
      describe={(row) => row.description || row.type || `Secret #${row.id}`}
      columns={[
        { field: "type", header: "Type" },
        { field: "description", header: "Description" },
        { field: "expiration", header: "Expires" },
      ]}
      emptyCreateDraft={emptyDraft}
      renderCreateForm={({ onCreated }) => {
        const selectedType = SECRET_TYPES.find((t) => t.value === draft.type) ?? SECRET_TYPES[0];
        const submit = async () => {
          if (!draft.type.trim() || !draft.value.trim()) return;
          const validationError = validateSecretValue(draft.type, draft.value);
          if (validationError) {
            setError(validationError);
            return;
          }
          setError(null);
          const r = await postApiV1ApiresourcesByApiResourceIdSecrets({
            path: { apiResourceId },
            body: { ...draft, expiration: expiresAt ? expiresAt.toISOString() : undefined },
          });
          if (r.error) {
            setError(problemMessage(r.error, "Failed to add secret."));
            return;
          }
          setDraft(emptyDraft);
          setExpiresAt(null);
          onCreated();
        };
        return (
          <div className="grid grid-cols-1 gap-3 md:grid-cols-2">
            <div>
              <label htmlFor="api-resource-secret-type" className="mb-1 block text-xs text-content-muted">
                Type
              </label>
              <Dropdown
                inputId="api-resource-secret-type"
                className="w-full"
                options={SECRET_TYPES}
                value={draft.type}
                onChange={(e) => setDraft({ ...draft, type: e.value })}
              />
            </div>
            <div className={selectedType.multiline ? "md:col-span-2" : undefined}>
              <label htmlFor="api-resource-secret-value" className="mb-1 block text-xs text-content-muted">
                Value{selectedType.valueHint}
              </label>
              {selectedType.multiline ? (
                <InputTextarea
                  id="api-resource-secret-value"
                  className="w-full"
                  rows={4}
                  value={draft.value}
                  placeholder={selectedType.placeholder}
                  onChange={(e) => setDraft({ ...draft, value: e.target.value })}
                />
              ) : (
                <InputText
                  id="api-resource-secret-value"
                  className="w-full"
                  value={draft.value}
                  placeholder={selectedType.placeholder}
                  onChange={(e) => setDraft({ ...draft, value: e.target.value })}
                />
              )}
            </div>
            <div>
              <label htmlFor="api-resource-secret-description" className="mb-1 block text-xs text-content-muted">
                Description (optional)
              </label>
              <InputText
                id="api-resource-secret-description"
                className="w-full"
                value={draft.description ?? ""}
                onChange={(e) => setDraft({ ...draft, description: e.target.value })}
              />
            </div>
            <div>
              <label htmlFor="api-resource-secret-expires" className="mb-1 block text-xs text-content-muted">
                Expires (optional)
              </label>
              <Calendar
                inputId="api-resource-secret-expires"
                className="w-full"
                value={expiresAt}
                onChange={(e) => setExpiresAt((e.value as Date | null) ?? null)}
                showTime
                showIcon
                hourFormat="24"
                dateFormat="yy-mm-dd"
                showButtonBar
              />
            </div>
            <div className="md:col-span-2">
              <Button
                type="button"
                onClick={submit}
                label="Add secret"
                icon={<FontAwesomeIcon icon={faPlus} />}
                disabled={!draft.type.trim() || !draft.value.trim()}
              />
            </div>
            {error && <p className="text-sm text-red-600 md:col-span-2">{error}</p>}
          </div>
        );
      }}
    />
  );
}
