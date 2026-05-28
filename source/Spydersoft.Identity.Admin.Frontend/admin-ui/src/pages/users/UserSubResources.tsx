import { useCallback, useEffect, useState } from "react";
import { DataTable } from "primereact/datatable";
import { Column } from "primereact/column";
import { Dropdown } from "primereact/dropdown";
import { Button } from "primereact/button";
import { ConfirmDialog, confirmDialog } from "primereact/confirmdialog";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faPlus, faTrash } from "@fortawesome/free-solid-svg-icons";

import {
  deleteApiV1UsersByIdRolesByRoleName,
  getApiV1Roles,
  getApiV1UsersByIdClaims,
  getApiV1UsersByIdRoles,
  postApiV1UsersByIdRoles,
} from "../../api/generated/sdk.gen";
import type {
  RoleSummaryDto,
  UserClaimDto,
  UserRoleDto,
} from "../../api/generated/types.gen";

interface PanelProps {
  userId: string;
}

export function RolesPanel({ userId }: PanelProps) {
  const [assigned, setAssigned] = useState<UserRoleDto[]>([]);
  const [allRoles, setAllRoles] = useState<RoleSummaryDto[]>([]);
  const [selected, setSelected] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);
  const [adding, setAdding] = useState(false);

  const refresh = useCallback(async () => {
    setLoading(true);
    const [rolesResult, allResult] = await Promise.all([
      getApiV1UsersByIdRoles({ path: { id: userId } }),
      getApiV1Roles(),
    ]);
    if (!rolesResult.error) setAssigned(rolesResult.data ?? []);
    if (!allResult.error) setAllRoles(allResult.data ?? []);
    setLoading(false);
  }, [userId]);

  useEffect(() => {
    void refresh();
  }, [refresh]);

  const assignedNames = new Set(assigned.map((r) => r.roleName));
  const available = allRoles
    .filter((r) => r.name && !assignedNames.has(r.name))
    .map((r) => ({ label: r.name as string, value: r.name as string }));

  const handleDelete = (row: UserRoleDto) => {
    if (!row.roleName) return;
    const roleName = row.roleName;
    confirmDialog({
      message: (
        <span>
          Remove role <strong>{roleName}</strong> from this user?
        </span>
      ),
      header: "Confirm removal",
      acceptClassName: "p-button-danger",
      acceptLabel: "Remove",
      rejectLabel: "Cancel",
      accept: async () => {
        await deleteApiV1UsersByIdRolesByRoleName({
          path: { id: userId, roleName },
        });
        await refresh();
      },
    });
  };

  const handleAdd = async () => {
    if (!selected) return;
    setAdding(true);
    try {
      await postApiV1UsersByIdRoles({
        path: { id: userId },
        body: { roleName: selected },
      });
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
          <h3 className="text-sm font-semibold text-content">Roles</h3>
          <p className="mt-0.5 text-xs text-content-muted">
            {loading ? "Loading…" : `${assigned.length} ${assigned.length === 1 ? "role" : "roles"}`}
          </p>
        </div>
      </header>

      <DataTable
        value={assigned}
        loading={loading}
        dataKey="roleName"
        emptyMessage="No roles assigned."
        size="small"
      >
        <Column field="roleName" header="Role" />
        <Column
          header=""
          style={{ width: "4.5rem" }}
          body={(row: UserRoleDto) => (
            <div className="flex justify-end">
              <Button
                text
                rounded
                size="small"
                severity="danger"
                aria-label="Remove"
                icon={<FontAwesomeIcon icon={faTrash} />}
                onClick={() => handleDelete(row)}
              />
            </div>
          )}
        />
      </DataTable>

      <div className="border-t border-border bg-surface-muted px-5 py-4">
        <h4 className="mb-3 text-xs font-semibold uppercase tracking-wider text-content-muted">
          Assign role
        </h4>
        <div className="flex gap-2">
          <Dropdown
            value={selected}
            options={available}
            onChange={(e) => setSelected(e.value as string)}
            placeholder={available.length === 0 ? "All roles already assigned" : "Select a role"}
            disabled={available.length === 0}
            className="flex-1"
          />
          <Button
            type="button"
            label="Add"
            icon={<FontAwesomeIcon icon={faPlus} />}
            disabled={!selected || adding}
            loading={adding}
            onClick={handleAdd}
          />
        </div>
      </div>
    </div>
  );
}

export function ClaimsPanel({ userId }: PanelProps) {
  const [claims, setClaims] = useState<UserClaimDto[]>([]);
  const [loading, setLoading] = useState(true);

  const refresh = useCallback(async () => {
    setLoading(true);
    const r = await getApiV1UsersByIdClaims({ path: { id: userId } });
    if (!r.error) setClaims(r.data ?? []);
    setLoading(false);
  }, [userId]);

  useEffect(() => {
    void refresh();
  }, [refresh]);

  return (
    <div className="overflow-hidden rounded-xl border border-border bg-surface shadow-sm">
      <header className="flex items-baseline justify-between gap-3 border-b border-border px-5 py-3.5">
        <div>
          <h3 className="text-sm font-semibold text-content">Claims</h3>
          <p className="mt-0.5 text-xs text-content-muted">
            {loading ? "Loading…" : `${claims.length} ${claims.length === 1 ? "claim" : "claims"} (read-only)`}
          </p>
        </div>
      </header>

      <DataTable
        value={claims}
        loading={loading}
        dataKey="type"
        emptyMessage="No claims."
        size="small"
      >
        <Column field="type" header="Type" />
        <Column field="value" header="Value" />
      </DataTable>
    </div>
  );
}
