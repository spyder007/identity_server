import { useCallback, useEffect, useState } from "react";
import { DataTable } from "primereact/datatable";
import { Column } from "primereact/column";
import { InputText } from "primereact/inputtext";
import { Dropdown } from "primereact/dropdown";
import { Button } from "primereact/button";
import { ConfirmDialog, confirmDialog } from "primereact/confirmdialog";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faPlus, faTrash } from "@fortawesome/free-solid-svg-icons";

import SubResourceList, { type SubResourceColumn } from "../../components/SubResourceList";
import {
  deleteApiV1ClientsByClientIdClaimsById,
  deleteApiV1ClientsByClientIdCorsoriginsById,
  deleteApiV1ClientsByClientIdGranttypesById,
  deleteApiV1ClientsByClientIdIdprestrictionsById,
  deleteApiV1ClientsByClientIdPostlogoutredirecturisById,
  deleteApiV1ClientsByClientIdPropertiesById,
  deleteApiV1ClientsByClientIdRedirecturisById,
  deleteApiV1ClientsByClientIdScopesById,
  deleteApiV1ClientsByClientIdSecretsById,
  getApiV1ClientsByClientIdClaims,
  getApiV1ClientsByClientIdCorsorigins,
  getApiV1ClientsByClientIdGranttypes,
  getApiV1ClientsByClientIdIdprestrictions,
  getApiV1ClientsByClientIdPostlogoutredirecturis,
  getApiV1ClientsByClientIdProperties,
  getApiV1ClientsByClientIdRedirecturis,
  getApiV1ClientsByClientIdScopes,
  getApiV1ClientsByClientIdSecrets,
  getApiV1IdentityResources,
  getApiV1Scopes,
  postApiV1ClientsByClientIdClaims,
  postApiV1ClientsByClientIdCorsorigins,
  postApiV1ClientsByClientIdGranttypes,
  postApiV1ClientsByClientIdIdprestrictions,
  postApiV1ClientsByClientIdPostlogoutredirecturis,
  postApiV1ClientsByClientIdProperties,
  postApiV1ClientsByClientIdRedirecturis,
  postApiV1ClientsByClientIdScopes,
  postApiV1ClientsByClientIdSecrets,
} from "../../api/generated/sdk.gen";
import type {
  ClientClaimDto,
  ClientCorsOriginDto,
  ClientGrantTypeDto,
  ClientIdpRestrictionDto,
  ClientPostLogoutRedirectUriDto,
  ClientPropertyDto,
  ClientRedirectUriDto,
  ClientScopeDto,
  ClientSecretDto,
  SaveClientSecretDto,
} from "../../api/generated/types.gen";

// Scopes are drawn from existing API scopes / identity resources plus the
// standard "offline_access" scope, since clients can't define new scopes here.
const STANDARD_SCOPES = ["offline_access"];

interface PanelProps {
  clientId: number;
}

// Shared single-value panel for sub-resources whose Save DTO is `{ <field>: string }`.
function SingleValuePanel<T extends { id?: number | string }>({
  title,
  emptyMessage,
  field,
  header,
  fieldType = "text",
  placeholder,
  load,
  create,
  remove,
  describe,
}: {
  title: string;
  emptyMessage: string;
  field: keyof T & string;
  header: string;
  fieldType?: "text" | "url";
  placeholder?: string;
  load: () => Promise<T[]>;
  create: (value: string) => Promise<void>;
  remove: (id: number) => Promise<void>;
  describe: (row: T) => string;
}) {
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
            <InputText
              type={fieldType}
              className="flex-1"
              value={createDraft.value}
              placeholder={placeholder}
              onChange={(e) => setCreateDraft({ value: e.target.value })}
              onKeyDown={(e) => e.key === "Enter" && submit()}
            />
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

// Two-field panel for {type, value} or {key, value} style sub-resources.
function KeyValuePanel<T extends { id?: number | string }>({
  title,
  emptyMessage,
  keyField,
  valueField,
  keyHeader,
  valueHeader,
  keyPlaceholder,
  valuePlaceholder,
  load,
  create,
  remove,
  describe,
}: {
  title: string;
  emptyMessage: string;
  keyField: keyof T & string;
  valueField: keyof T & string;
  keyHeader: string;
  valueHeader: string;
  keyPlaceholder?: string;
  valuePlaceholder?: string;
  load: () => Promise<T[]>;
  create: (key: string, value: string) => Promise<void>;
  remove: (id: number) => Promise<void>;
  describe: (row: T) => string;
}) {
  const columns: SubResourceColumn<T>[] = [
    { field: keyField, header: keyHeader },
    { field: valueField, header: valueHeader },
  ];

  return (
    <SubResourceList<T, { key: string; value: string }>
      title={title}
      emptyMessage={emptyMessage}
      load={load}
      remove={remove}
      describe={describe}
      columns={columns}
      emptyCreateDraft={{ key: "", value: "" }}
      renderCreateForm={({ onCreated, createDraft, setCreateDraft }) => {
        const submit = async () => {
          if (!createDraft.key.trim() || !createDraft.value.trim()) return;
          await create(createDraft.key.trim(), createDraft.value.trim());
          onCreated();
        };
        return (
          <div className="flex gap-2">
            <InputText
              className="flex-1"
              value={createDraft.key}
              placeholder={keyPlaceholder}
              onChange={(e) => setCreateDraft({ ...createDraft, key: e.target.value })}
              onKeyDown={(e) => e.key === "Enter" && submit()}
            />
            <InputText
              className="flex-1"
              value={createDraft.value}
              placeholder={valuePlaceholder}
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

export function ClaimsPanel({ clientId }: PanelProps) {
  return (
    <KeyValuePanel<ClientClaimDto>
      title="Client claims"
      emptyMessage="No claims defined."
      keyField="type"
      valueField="value"
      keyHeader="Type"
      valueHeader="Value"
      keyPlaceholder="claim_type"
      valuePlaceholder="value"
      load={async () => {
        const r = await getApiV1ClientsByClientIdClaims({ path: { clientId } });
        return r.error ? [] : (r.data ?? []);
      }}
      create={async (type, value) => {
        await postApiV1ClientsByClientIdClaims({ path: { clientId }, body: { type, value } });
      }}
      remove={async (id) => {
        await deleteApiV1ClientsByClientIdClaimsById({ path: { clientId, id } });
      }}
      describe={(row) => `${row.type ?? ""} = ${row.value ?? ""}`}
    />
  );
}

export function CorsOriginsPanel({ clientId }: PanelProps) {
  return (
    <SingleValuePanel<ClientCorsOriginDto>
      title="CORS origins"
      emptyMessage="No CORS origins configured."
      field="origin"
      header="Origin"
      fieldType="url"
      placeholder="https://app.example.com"
      load={async () => {
        const r = await getApiV1ClientsByClientIdCorsorigins({ path: { clientId } });
        return r.error ? [] : (r.data ?? []);
      }}
      create={async (origin) => {
        await postApiV1ClientsByClientIdCorsorigins({ path: { clientId }, body: { origin } });
      }}
      remove={async (id) => {
        await deleteApiV1ClientsByClientIdCorsoriginsById({ path: { clientId, id } });
      }}
      describe={(row) => row.origin ?? ""}
    />
  );
}

export function GrantTypesPanel({ clientId }: PanelProps) {
  return (
    <SingleValuePanel<ClientGrantTypeDto>
      title="Grant types"
      emptyMessage="No grant types configured."
      field="grantType"
      header="Grant type"
      placeholder="authorization_code"
      load={async () => {
        const r = await getApiV1ClientsByClientIdGranttypes({ path: { clientId } });
        return r.error ? [] : (r.data ?? []);
      }}
      create={async (grantType) => {
        await postApiV1ClientsByClientIdGranttypes({ path: { clientId }, body: { grantType } });
      }}
      remove={async (id) => {
        await deleteApiV1ClientsByClientIdGranttypesById({ path: { clientId, id } });
      }}
      describe={(row) => row.grantType ?? ""}
    />
  );
}

export function IdpRestrictionsPanel({ clientId }: PanelProps) {
  return (
    <SingleValuePanel<ClientIdpRestrictionDto>
      title="Identity provider restrictions"
      emptyMessage="No restrictions configured (any IdP allowed)."
      field="provider"
      header="Provider"
      placeholder="Google"
      load={async () => {
        const r = await getApiV1ClientsByClientIdIdprestrictions({ path: { clientId } });
        return r.error ? [] : (r.data ?? []);
      }}
      create={async (provider) => {
        await postApiV1ClientsByClientIdIdprestrictions({ path: { clientId }, body: { provider } });
      }}
      remove={async (id) => {
        await deleteApiV1ClientsByClientIdIdprestrictionsById({ path: { clientId, id } });
      }}
      describe={(row) => row.provider ?? ""}
    />
  );
}

export function PostLogoutRedirectUrisPanel({ clientId }: PanelProps) {
  return (
    <SingleValuePanel<ClientPostLogoutRedirectUriDto>
      title="Post-logout redirect URIs"
      emptyMessage="No URIs configured."
      field="postLogoutRedirectUri"
      header="URI"
      fieldType="url"
      placeholder="https://app.example.com/logout-callback"
      load={async () => {
        const r = await getApiV1ClientsByClientIdPostlogoutredirecturis({ path: { clientId } });
        return r.error ? [] : (r.data ?? []);
      }}
      create={async (postLogoutRedirectUri) => {
        await postApiV1ClientsByClientIdPostlogoutredirecturis({
          path: { clientId },
          body: { postLogoutRedirectUri },
        });
      }}
      remove={async (id) => {
        await deleteApiV1ClientsByClientIdPostlogoutredirecturisById({ path: { clientId, id } });
      }}
      describe={(row) => row.postLogoutRedirectUri ?? ""}
    />
  );
}

export function PropertiesPanel({ clientId }: PanelProps) {
  return (
    <KeyValuePanel<ClientPropertyDto>
      title="Custom properties"
      emptyMessage="No custom properties."
      keyField="key"
      valueField="value"
      keyHeader="Key"
      valueHeader="Value"
      load={async () => {
        const r = await getApiV1ClientsByClientIdProperties({ path: { clientId } });
        return r.error ? [] : (r.data ?? []);
      }}
      create={async (key, value) => {
        await postApiV1ClientsByClientIdProperties({ path: { clientId }, body: { key, value } });
      }}
      remove={async (id) => {
        await deleteApiV1ClientsByClientIdPropertiesById({ path: { clientId, id } });
      }}
      describe={(row) => `${row.key ?? ""} = ${row.value ?? ""}`}
    />
  );
}

export function RedirectUrisPanel({ clientId }: PanelProps) {
  return (
    <SingleValuePanel<ClientRedirectUriDto>
      title="Redirect URIs"
      emptyMessage="No redirect URIs configured."
      field="redirectUri"
      header="URI"
      fieldType="url"
      placeholder="https://app.example.com/signin-oidc"
      load={async () => {
        const r = await getApiV1ClientsByClientIdRedirecturis({ path: { clientId } });
        return r.error ? [] : (r.data ?? []);
      }}
      create={async (redirectUri) => {
        await postApiV1ClientsByClientIdRedirecturis({ path: { clientId }, body: { redirectUri } });
      }}
      remove={async (id) => {
        await deleteApiV1ClientsByClientIdRedirecturisById({ path: { clientId, id } });
      }}
      describe={(row) => row.redirectUri ?? ""}
    />
  );
}

export function ScopesPanel({ clientId }: PanelProps) {
  const [assigned, setAssigned] = useState<ClientScopeDto[]>([]);
  const [allScopeNames, setAllScopeNames] = useState<string[]>([]);
  const [selected, setSelected] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);
  const [adding, setAdding] = useState(false);

  const refresh = useCallback(async () => {
    setLoading(true);
    const [assignedResult, apiScopesResult, identityResourcesResult] = await Promise.all([
      getApiV1ClientsByClientIdScopes({ path: { clientId } }),
      getApiV1Scopes(),
      getApiV1IdentityResources(),
    ]);
    if (!assignedResult.error) setAssigned(assignedResult.data ?? []);
    const names = new Set(STANDARD_SCOPES);
    for (const s of apiScopesResult.data ?? []) if (s.name) names.add(s.name);
    for (const r of identityResourcesResult.data ?? []) if (r.name) names.add(r.name);
    setAllScopeNames([...names].sort((a, b) => a.localeCompare(b)));
    setLoading(false);
  }, [clientId]);

  useEffect(() => {
    void refresh();
  }, [refresh]);

  const assignedNames = new Set(assigned.map((s) => s.scope));
  const available = allScopeNames
    .filter((name) => !assignedNames.has(name))
    .map((name) => ({ label: name, value: name }));

  const handleDelete = (row: ClientScopeDto) => {
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
        await deleteApiV1ClientsByClientIdScopesById({ path: { clientId, id: Number(row.id) } });
        await refresh();
      },
    });
  };

  const handleAdd = async () => {
    if (!selected) return;
    setAdding(true);
    try {
      await postApiV1ClientsByClientIdScopes({ path: { clientId }, body: { scope: selected } });
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
          <h3 className="text-sm font-semibold text-content">Allowed scopes</h3>
          <p className="mt-0.5 text-xs text-content-muted">
            {loading ? "Loading…" : `${assigned.length} ${assigned.length === 1 ? "entry" : "entries"}`}
          </p>
        </div>
      </header>

      <DataTable value={assigned} loading={loading} dataKey="id" emptyMessage="No scopes configured." size="small">
        <Column field="scope" header="Scope" />
        <Column
          header=""
          style={{ width: "4.5rem" }}
          body={(row: ClientScopeDto) => (
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
      </div>
    </div>
  );
}

// Secrets get their own panel — the create form has more fields (type, value, description, expiration).
export function SecretsPanel({ clientId }: PanelProps) {
  const [draft, setDraft] = useState<SaveClientSecretDto>({ type: "SharedSecret", value: "" });
  const emptyDraft: SaveClientSecretDto = { type: "SharedSecret", value: "" };

  return (
    <SubResourceList<ClientSecretDto, SaveClientSecretDto>
      title="Client secrets"
      emptyMessage="No secrets configured."
      load={async () => {
        const r = await getApiV1ClientsByClientIdSecrets({ path: { clientId } });
        return r.error ? [] : (r.data ?? []);
      }}
      remove={async (id) => {
        await deleteApiV1ClientsByClientIdSecretsById({ path: { clientId, id } });
      }}
      describe={(row) => row.description || row.type || `Secret #${row.id}`}
      columns={[
        { field: "type", header: "Type" },
        { field: "description", header: "Description" },
        { field: "expiration", header: "Expires" },
      ]}
      emptyCreateDraft={emptyDraft}
      renderCreateForm={({ onCreated }) => {
        const submit = async () => {
          if (!draft.type.trim() || !draft.value.trim()) return;
          await postApiV1ClientsByClientIdSecrets({ path: { clientId }, body: draft });
          setDraft(emptyDraft);
          onCreated();
        };
        return (
          <div className="grid grid-cols-1 gap-3 md:grid-cols-2">
            <div>
              <label className="mb-1 block text-xs text-content-muted">Type</label>
              <InputText className="w-full" value={draft.type} onChange={(e) => setDraft({ ...draft, type: e.target.value })} />
            </div>
            <div>
              <label className="mb-1 block text-xs text-content-muted">Value (will be SHA-256 hashed)</label>
              <InputText className="w-full" value={draft.value} onChange={(e) => setDraft({ ...draft, value: e.target.value })} />
            </div>
            <div>
              <label className="mb-1 block text-xs text-content-muted">Description (optional)</label>
              <InputText className="w-full" value={draft.description ?? ""} onChange={(e) => setDraft({ ...draft, description: e.target.value })} />
            </div>
            <div>
              <label className="mb-1 block text-xs text-content-muted">Expires (ISO date, optional)</label>
              <InputText className="w-full" value={draft.expiration ?? ""} onChange={(e) => setDraft({ ...draft, expiration: e.target.value })} placeholder="2027-01-01T00:00:00Z" />
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
          </div>
        );
      }}
    />
  );
}
