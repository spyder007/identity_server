import { useState } from "react";
import { InputText } from "primereact/inputtext";
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
      renderCreateForm={({ onCreated, createDraft, setCreateDraft }) => (
        <form
          className="flex gap-2"
          onSubmit={async (e) => {
            e.preventDefault();
            if (!createDraft.value.trim()) return;
            await create(createDraft.value.trim());
            onCreated();
          }}
        >
          <InputText
            className="flex-1"
            value={createDraft.value}
            placeholder={placeholder}
            onChange={(e) => setCreateDraft({ value: e.target.value })}
          />
          <Button
            type="submit"
            label="Add"
            icon={<FontAwesomeIcon icon={faPlus} />}
            disabled={!createDraft.value.trim()}
          />
        </form>
      )}
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
      renderCreateForm={({ onCreated, createDraft, setCreateDraft }) => (
        <form
          className="flex gap-2"
          onSubmit={async (e) => {
            e.preventDefault();
            if (!createDraft.key.trim() || !createDraft.value.trim()) return;
            await postApiV1ApiresourcesByApiResourceIdProperties({
              path: { apiResourceId },
              body: { key: createDraft.key.trim(), value: createDraft.value.trim() },
            });
            onCreated();
          }}
        >
          <InputText
            className="flex-1"
            value={createDraft.key}
            placeholder="key"
            onChange={(e) => setCreateDraft({ ...createDraft, key: e.target.value })}
          />
          <InputText
            className="flex-1"
            value={createDraft.value}
            placeholder="value"
            onChange={(e) => setCreateDraft({ ...createDraft, value: e.target.value })}
          />
          <Button
            type="submit"
            label="Add"
            icon={<FontAwesomeIcon icon={faPlus} />}
            disabled={!createDraft.key.trim() || !createDraft.value.trim()}
          />
        </form>
      )}
    />
  );
}

export function SecretsPanel({ apiResourceId }: PanelProps) {
  const emptyDraft: SaveApiResourceSecretDto = { type: "SharedSecret", value: "" };
  const [draft, setDraft] = useState<SaveApiResourceSecretDto>(emptyDraft);

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
      renderCreateForm={({ onCreated }) => (
        <form
          className="grid grid-cols-1 gap-3 md:grid-cols-2"
          onSubmit={async (e) => {
            e.preventDefault();
            if (!draft.type.trim() || !draft.value.trim()) return;
            await postApiV1ApiresourcesByApiResourceIdSecrets({
              path: { apiResourceId },
              body: draft,
            });
            setDraft(emptyDraft);
            onCreated();
          }}
        >
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
              type="submit"
              label="Add secret"
              icon={<FontAwesomeIcon icon={faPlus} />}
              disabled={!draft.type.trim() || !draft.value.trim()}
            />
          </div>
        </form>
      )}
    />
  );
}
