import { InputText } from "primereact/inputtext";
import { Button } from "primereact/button";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faPlus } from "@fortawesome/free-solid-svg-icons";

import SubResourceList, { type SubResourceColumn } from "../../components/SubResourceList";
import {
  deleteApiV1IdentityresourcesByIdentityResourceIdClaimsById,
  deleteApiV1IdentityresourcesByIdentityResourceIdPropertiesById,
  getApiV1IdentityresourcesByIdentityResourceIdClaims,
  getApiV1IdentityresourcesByIdentityResourceIdProperties,
  postApiV1IdentityresourcesByIdentityResourceIdClaims,
  postApiV1IdentityresourcesByIdentityResourceIdProperties,
} from "../../api/generated/sdk.gen";
import type {
  IdentityResourceClaimDto,
  IdentityResourcePropertyDto,
} from "../../api/generated/types.gen";

interface PanelProps {
  identityResourceId: number;
}

export function ClaimsPanel({ identityResourceId }: PanelProps) {
  const columns: SubResourceColumn<IdentityResourceClaimDto>[] = [
    { field: "type", header: "Type" },
  ];

  return (
    <SubResourceList<IdentityResourceClaimDto, { value: string }>
      title="User claims"
      emptyMessage="No user claims defined."
      load={async () => {
        const r = await getApiV1IdentityresourcesByIdentityResourceIdClaims({
          path: { identityResourceId },
        });
        return r.error ? [] : (r.data ?? []);
      }}
      remove={async (id) => {
        await deleteApiV1IdentityresourcesByIdentityResourceIdClaimsById({
          path: { identityResourceId, id },
        });
      }}
      describe={(row) => row.type ?? ""}
      columns={columns}
      emptyCreateDraft={{ value: "" }}
      renderCreateForm={({ onCreated, createDraft, setCreateDraft }) => (
        <form
          className="flex gap-2"
          onSubmit={async (e) => {
            e.preventDefault();
            if (!createDraft.value.trim()) return;
            await postApiV1IdentityresourcesByIdentityResourceIdClaims({
              path: { identityResourceId },
              body: { type: createDraft.value.trim() },
            });
            onCreated();
          }}
        >
          <InputText
            className="flex-1"
            value={createDraft.value}
            placeholder="email"
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

export function PropertiesPanel({ identityResourceId }: PanelProps) {
  return (
    <SubResourceList<IdentityResourcePropertyDto, { key: string; value: string }>
      title="Custom properties"
      emptyMessage="No custom properties."
      load={async () => {
        const r = await getApiV1IdentityresourcesByIdentityResourceIdProperties({
          path: { identityResourceId },
        });
        return r.error ? [] : (r.data ?? []);
      }}
      remove={async (id) => {
        await deleteApiV1IdentityresourcesByIdentityResourceIdPropertiesById({
          path: { identityResourceId, id },
        });
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
            await postApiV1IdentityresourcesByIdentityResourceIdProperties({
              path: { identityResourceId },
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
