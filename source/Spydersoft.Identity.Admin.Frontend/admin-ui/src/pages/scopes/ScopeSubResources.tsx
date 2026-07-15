import { InputText } from "primereact/inputtext";
import { Button } from "primereact/button";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faPlus } from "@fortawesome/free-solid-svg-icons";

import SubResourceList, { type SubResourceColumn } from "../../components/SubResourceList";
import {
  deleteApiV1ScopesByScopeIdClaimsById,
  deleteApiV1ScopesByScopeIdPropertiesById,
  getApiV1ScopesByScopeIdClaims,
  getApiV1ScopesByScopeIdProperties,
  postApiV1ScopesByScopeIdClaims,
  postApiV1ScopesByScopeIdProperties,
} from "../../api/generated/sdk.gen";
import type {
  ScopeClaimDto,
  ScopePropertyDto,
} from "../../api/generated/types.gen";

interface PanelProps {
  scopeId: number;
}

export function ClaimsPanel({ scopeId }: PanelProps) {
  const columns: SubResourceColumn<ScopeClaimDto>[] = [
    { field: "type", header: "Type" },
  ];

  return (
    <SubResourceList<ScopeClaimDto, { value: string }>
      title="User claims"
      emptyMessage="No user claims defined."
      load={async () => {
        const r = await getApiV1ScopesByScopeIdClaims({ path: { scopeId } });
        return r.error ? [] : (r.data ?? []);
      }}
      remove={async (id) => {
        await deleteApiV1ScopesByScopeIdClaimsById({ path: { scopeId, id } });
      }}
      describe={(row) => row.type ?? ""}
      columns={columns}
      emptyCreateDraft={{ value: "" }}
      renderCreateForm={({ onCreated, createDraft, setCreateDraft }) => {
        const submit = async () => {
          if (!createDraft.value.trim()) return;
          await postApiV1ScopesByScopeIdClaims({
            path: { scopeId },
            body: { type: createDraft.value.trim() },
          });
          onCreated();
        };
        return (
          <div className="flex gap-2">
            <InputText
              className="flex-1"
              value={createDraft.value}
              placeholder="email"
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

export function PropertiesPanel({ scopeId }: PanelProps) {
  return (
    <SubResourceList<ScopePropertyDto, { key: string; value: string }>
      title="Custom properties"
      emptyMessage="No custom properties."
      load={async () => {
        const r = await getApiV1ScopesByScopeIdProperties({ path: { scopeId } });
        return r.error ? [] : (r.data ?? []);
      }}
      remove={async (id) => {
        await deleteApiV1ScopesByScopeIdPropertiesById({ path: { scopeId, id } });
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
          await postApiV1ScopesByScopeIdProperties({
            path: { scopeId },
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
