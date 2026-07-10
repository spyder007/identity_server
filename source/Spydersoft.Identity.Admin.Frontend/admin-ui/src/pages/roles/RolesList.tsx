import { useCallback, useEffect, useMemo, useRef, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { DataTable } from "primereact/datatable";
import { Column } from "primereact/column";
import { InputText } from "primereact/inputtext";
import { Button } from "primereact/button";
import { ConfirmDialog, confirmDialog } from "primereact/confirmdialog";
import { Toast } from "primereact/toast";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  faMagnifyingGlass,
  faPencil,
  faPlus,
  faTrash,
  faUserShield,
} from "@fortawesome/free-solid-svg-icons";

import PageHeader from "../../components/PageHeader";
import EmptyState from "../../components/EmptyState";
import {
  deleteApiV1RolesById,
  getApiV1Roles,
} from "../../api/generated/sdk.gen";
import type { RoleSummaryDto } from "../../api/generated/types.gen";

export default function RolesList() {
  const navigate = useNavigate();
  const toast = useRef<Toast>(null);
  const [roles, setRoles] = useState<RoleSummaryDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [filter, setFilter] = useState("");

  const load = useCallback(async () => {
    setLoading(true);
    const result = await getApiV1Roles();
    if (!result.error) setRoles(result.data ?? []);
    setLoading(false);
  }, []);

  useEffect(() => {
    void load();
  }, [load]);

  const filtered = useMemo(() => {
    if (!filter) return roles;
    const f = filter.toLowerCase();
    return roles.filter((r) => (r.name ?? "").toLowerCase().includes(f));
  }, [roles, filter]);

  const handleDelete = (role: RoleSummaryDto) => {
    confirmDialog({
      message: (
        <span>
          Delete role <strong>{role.name}</strong>? This cannot be undone.
        </span>
      ),
      header: "Confirm deletion",
      acceptClassName: "p-button-danger",
      acceptLabel: "Delete",
      rejectLabel: "Cancel",
      accept: async () => {
        if (!role.id) return;
        const result = await deleteApiV1RolesById({ path: { id: role.id } });
        if (!result.error) {
          toast.current?.show({
            severity: "success",
            summary: "Deleted",
            detail: `Role "${role.name}" removed.`,
            life: 3000,
          });
          await load();
        }
      },
    });
  };

  const newRoleButton = (
    <Link to="new">
      <Button label="New role" icon={<FontAwesomeIcon icon={faPlus} />} />
    </Link>
  );

  const showEmptyState = !loading && roles.length === 0;

  return (
    <div>
      <Toast ref={toast} position="top-right" />
      <ConfirmDialog />
      <PageHeader
        icon={faUserShield}
        eyebrow="People"
        title="Roles"
        subtitle="Named groups that grant claims to assigned users"
        actions={newRoleButton}
      />

      {showEmptyState ? (
        <EmptyState
          icon={faUserShield}
          title="No roles yet"
          description="Define roles to group users and attach claims."
          action={newRoleButton}
        />
      ) : (
        <div className="overflow-hidden rounded-xl border border-border bg-surface shadow-sm">
          <div className="flex flex-wrap items-center justify-between gap-3 border-b border-border px-4 py-3">
            <span className="text-sm text-content-muted">
              {filtered.length} of {roles.length} {roles.length === 1 ? "role" : "roles"}
            </span>
            <div className="relative">
              <FontAwesomeIcon
                icon={faMagnifyingGlass}
                className="pointer-events-none absolute left-3 top-1/2 -translate-y-1/2 text-xs text-content-subtle"
              />
              <InputText
                value={filter}
                onChange={(e) => setFilter(e.target.value)}
                placeholder="Search by name"
                className="pl-8!"
              />
            </div>
          </div>

          <DataTable
            value={filtered}
            loading={loading}
            dataKey="id"
            paginator
            rows={20}
            rowsPerPageOptions={[10, 20, 50]}
            emptyMessage="No roles match your search."
            onRowClick={(e) => {
              const id = (e.data as RoleSummaryDto).id;
              if (id) navigate(id);
            }}
            rowClassName={() => "cursor-pointer"}
          >
            <Column
              field="name"
              header="Name"
              sortable
              body={(row: RoleSummaryDto) => (
                <span className="font-mono text-[0.85rem] text-content">{row.name}</span>
              )}
            />
            <Column
              header=""
              body={(row: RoleSummaryDto) => (
                <div className="flex justify-end gap-1">
                  {row.id && (
                    <Link to={row.id} onClick={(e) => e.stopPropagation()}>
                      <Button
                        text
                        rounded
                        size="small"
                        aria-label="Edit"
                        icon={<FontAwesomeIcon icon={faPencil} />}
                      />
                    </Link>
                  )}
                  <Button
                    text
                    rounded
                    size="small"
                    severity="danger"
                    aria-label="Delete"
                    icon={<FontAwesomeIcon icon={faTrash} />}
                    onClick={(e) => {
                      e.stopPropagation();
                      handleDelete(row);
                    }}
                  />
                </div>
              )}
              style={{ width: "7rem" }}
            />
          </DataTable>
        </div>
      )}
    </div>
  );
}
