import { useState } from "react";
import { InputText } from "primereact/inputtext";
import { InputTextarea } from "primereact/inputtextarea";
import { Dropdown } from "primereact/dropdown";
import { Calendar } from "primereact/calendar";
import { Button } from "primereact/button";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faPlus } from "@fortawesome/free-solid-svg-icons";

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
  load,
  create,
  remove,
  describe,
}: {
  title: string;
  emptyMessage: string;
  field: keyof T & string;
  header: string;
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

export function ClaimsPanel({ apiResourceId }: PanelProps) {
  return (
    <SingleValuePanel<ApiResourceClaimDto>
      title="User claims"
      emptyMessage="No user claims defined."
      field="type"
      header="Type"
      placeholder="email"
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

export function ScopesPanel({ apiResourceId }: PanelProps) {
  return (
    <SingleValuePanel<ApiResourceScopeDto>
      title="Scopes"
      emptyMessage="No scopes associated with this API."
      field="scope"
      header="Scope"
      placeholder="my.api.read"
      load={async () => {
        const r = await getApiV1ApiresourcesByApiResourceIdScopes({ path: { apiResourceId } });
        return r.error ? [] : (r.data ?? []);
      }}
      create={async (scope) => {
        await postApiV1ApiresourcesByApiResourceIdScopes({
          path: { apiResourceId },
          body: { scope },
        });
      }}
      remove={async (id) => {
        await deleteApiV1ApiresourcesByApiResourceIdScopesById({ path: { apiResourceId, id } });
      }}
      describe={(row) => row.scope ?? ""}
    />
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
