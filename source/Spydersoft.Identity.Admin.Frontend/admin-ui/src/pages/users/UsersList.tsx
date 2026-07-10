import { useCallback, useEffect, useMemo, useRef, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { DataTable } from "primereact/datatable";
import { Column } from "primereact/column";
import { InputText } from "primereact/inputtext";
import { Button } from "primereact/button";
import { Tag } from "primereact/tag";
import { ConfirmDialog, confirmDialog } from "primereact/confirmdialog";
import { Toast } from "primereact/toast";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  faMagnifyingGlass,
  faPencil,
  faPlus,
  faTrash,
  faUsers,
} from "@fortawesome/free-solid-svg-icons";

import PageHeader from "../../components/PageHeader";
import EmptyState from "../../components/EmptyState";
import {
  deleteApiV1UsersById,
  getApiV1Users,
} from "../../api/generated/sdk.gen";
import type { UserSummaryDto } from "../../api/generated/types.gen";

export default function UsersList() {
  const navigate = useNavigate();
  const toast = useRef<Toast>(null);
  const [users, setUsers] = useState<UserSummaryDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [filter, setFilter] = useState("");

  const load = useCallback(async () => {
    setLoading(true);
    const result = await getApiV1Users();
    if (!result.error) setUsers(result.data ?? []);
    setLoading(false);
  }, []);

  useEffect(() => {
    void load();
  }, [load]);

  const filtered = useMemo(() => {
    if (!filter) return users;
    const f = filter.toLowerCase();
    return users.filter(
      (u) =>
        (u.userName ?? "").toLowerCase().includes(f) ||
        (u.email ?? "").toLowerCase().includes(f) ||
        (u.name ?? "").toLowerCase().includes(f),
    );
  }, [users, filter]);

  const handleDelete = (user: UserSummaryDto) => {
    confirmDialog({
      message: (
        <span>
          Delete user <strong>{user.userName || user.email}</strong>? This cannot be undone.
        </span>
      ),
      header: "Confirm deletion",
      acceptClassName: "p-button-danger",
      acceptLabel: "Delete",
      rejectLabel: "Cancel",
      accept: async () => {
        if (!user.id) return;
        const result = await deleteApiV1UsersById({ path: { id: user.id } });
        if (!result.error) {
          toast.current?.show({
            severity: "success",
            summary: "Deleted",
            detail: `User "${user.userName || user.email}" removed.`,
            life: 3000,
          });
          await load();
        }
      },
    });
  };

  const newUserButton = (
    <Link to="new">
      <Button label="New user" icon={<FontAwesomeIcon icon={faPlus} />} />
    </Link>
  );

  const showEmptyState = !loading && users.length === 0;

  return (
    <div>
      <Toast ref={toast} position="top-right" />
      <ConfirmDialog />
      <PageHeader
        icon={faUsers}
        eyebrow="People"
        title="Users"
        subtitle="Accounts that can authenticate with this identity server"
        actions={newUserButton}
      />

      {showEmptyState ? (
        <EmptyState
          icon={faUsers}
          title="No users yet"
          description="Create the first user account."
          action={newUserButton}
        />
      ) : (
        <div className="overflow-hidden rounded-xl border border-border bg-surface shadow-sm">
          <div className="flex flex-wrap items-center justify-between gap-3 border-b border-border px-4 py-3">
            <span className="text-sm text-content-muted">
              {filtered.length} of {users.length} {users.length === 1 ? "user" : "users"}
            </span>
            <div className="relative">
              <FontAwesomeIcon
                icon={faMagnifyingGlass}
                className="pointer-events-none absolute left-3 top-1/2 -translate-y-1/2 text-xs text-content-subtle"
              />
              <InputText
                value={filter}
                onChange={(e) => setFilter(e.target.value)}
                placeholder="Search by name or email"
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
            emptyMessage="No users match your search."
            onRowClick={(e) => {
              const id = (e.data as UserSummaryDto).id;
              if (id) navigate(id);
            }}
            rowClassName={() => "cursor-pointer"}
          >
            <Column
              field="userName"
              header="Username"
              sortable
              body={(row: UserSummaryDto) => (
                <span className="font-mono text-[0.85rem] text-content">{row.userName}</span>
              )}
            />
            <Column
              field="email"
              header="Email"
              sortable
              body={(row: UserSummaryDto) =>
                row.email ?? <span className="text-content-subtle">—</span>
              }
            />
            <Column
              field="name"
              header="Name"
              sortable
              body={(row: UserSummaryDto) =>
                row.name ?? <span className="text-content-subtle">—</span>
              }
            />
            <Column
              header="Status"
              body={(row: UserSummaryDto) => (
                <div className="flex flex-wrap gap-1">
                  {row.emailConfirmed ? (
                    <Tag severity="success" value="Email verified" />
                  ) : (
                    <Tag severity="warning" value="Email unverified" />
                  )}
                  {row.twoFactorEnabled && <Tag severity="info" value="2FA" />}
                  {row.lockoutEnabled && <Tag severity="danger" value="Lockout" />}
                </div>
              )}
            />
            <Column
              header=""
              body={(row: UserSummaryDto) => (
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
