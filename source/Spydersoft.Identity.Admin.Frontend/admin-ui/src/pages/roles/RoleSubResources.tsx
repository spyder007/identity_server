import { useCallback, useEffect, useState } from "react";
import { DataTable } from "primereact/datatable";
import { Column } from "primereact/column";
import { InputText } from "primereact/inputtext";
import { Button } from "primereact/button";
import { ConfirmDialog, confirmDialog } from "primereact/confirmdialog";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faPlus, faTrash } from "@fortawesome/free-solid-svg-icons";

import {
  deleteApiV1RolesByIdClaimsByClaimType,
  getApiV1RolesByIdClaims,
  postApiV1RolesByIdClaims,
} from "../../api/generated/sdk.gen";
import type { RoleClaimDto } from "../../api/generated/types.gen";

interface PanelProps {
  roleId: string;
}

export function ClaimsPanel({ roleId }: PanelProps) {
  const [claims, setClaims] = useState<RoleClaimDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [type, setType] = useState("");
  const [value, setValue] = useState("");
  const [adding, setAdding] = useState(false);

  const refresh = useCallback(async () => {
    setLoading(true);
    const r = await getApiV1RolesByIdClaims({ path: { id: roleId } });
    if (!r.error) setClaims(r.data ?? []);
    setLoading(false);
  }, [roleId]);

  useEffect(() => {
    void refresh();
  }, [refresh]);

  const handleDelete = (row: RoleClaimDto) => {
    if (!row.type) return;
    const claimType = row.type;
    confirmDialog({
      message: (
        <span>
          Delete claim <strong>{claimType}</strong>?
        </span>
      ),
      header: "Confirm deletion",
      acceptClassName: "p-button-danger",
      acceptLabel: "Delete",
      rejectLabel: "Cancel",
      accept: async () => {
        await deleteApiV1RolesByIdClaimsByClaimType({
          path: { id: roleId, claimType },
        });
        await refresh();
      },
    });
  };

  const handleAdd = async () => {
    if (!type.trim()) return;
    setAdding(true);
    try {
      await postApiV1RolesByIdClaims({
        path: { id: roleId },
        body: { type: type.trim(), value: value.trim() },
      });
      setType("");
      setValue("");
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
          <h3 className="text-sm font-semibold text-content">Role claims</h3>
          <p className="mt-0.5 text-xs text-content-muted">
            {loading ? "Loading…" : `${claims.length} ${claims.length === 1 ? "claim" : "claims"}`}
          </p>
        </div>
      </header>

      <DataTable
        value={claims}
        loading={loading}
        dataKey="type"
        emptyMessage="No claims defined."
        size="small"
      >
        <Column field="type" header="Type" />
        <Column field="value" header="Value" />
        <Column
          header=""
          style={{ width: "4.5rem" }}
          body={(row: RoleClaimDto) => (
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
          Add claim
        </h4>
        <div className="flex gap-2">
          <InputText
            className="flex-1"
            value={type}
            placeholder="type"
            onChange={(e) => setType(e.target.value)}
            onKeyDown={(e) => e.key === "Enter" && handleAdd()}
          />
          <InputText
            className="flex-1"
            value={value}
            placeholder="value"
            onChange={(e) => setValue(e.target.value)}
            onKeyDown={(e) => e.key === "Enter" && handleAdd()}
          />
          <Button
            type="button"
            onClick={handleAdd}
            label="Add"
            icon={<FontAwesomeIcon icon={faPlus} />}
            disabled={!type.trim() || adding}
            loading={adding}
          />
        </div>
      </div>
    </div>
  );
}
