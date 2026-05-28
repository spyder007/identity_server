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
  faShield,
  faTrash,
} from "@fortawesome/free-solid-svg-icons";

import PageHeader from "../../components/PageHeader";
import EmptyState from "../../components/EmptyState";
import { formatDate, asId } from "../../utils/format";
import {
  deleteApiV1IdentityResourcesById,
  getApiV1IdentityResources,
} from "../../api/generated/sdk.gen";
import type { IdentityResourceSummaryDto } from "../../api/generated/types.gen";

export default function IdentityResourcesList() {
  const navigate = useNavigate();
  const toast = useRef<Toast>(null);
  const [resources, setResources] = useState<IdentityResourceSummaryDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [filter, setFilter] = useState("");

  const load = useCallback(async () => {
    setLoading(true);
    const result = await getApiV1IdentityResources();
    if (!result.error) setResources(result.data ?? []);
    setLoading(false);
  }, []);

  useEffect(() => {
    void load();
  }, [load]);

  const filtered = useMemo(() => {
    if (!filter) return resources;
    const f = filter.toLowerCase();
    return resources.filter(
      (r) =>
        (r.name ?? "").toLowerCase().includes(f) ||
        (r.displayName ?? "").toLowerCase().includes(f),
    );
  }, [resources, filter]);

  const handleDelete = (resource: IdentityResourceSummaryDto) => {
    confirmDialog({
      message: (
        <span>
          Delete identity resource <strong>{resource.displayName || resource.name}</strong>? This cannot be undone.
        </span>
      ),
      header: "Confirm deletion",
      acceptClassName: "p-button-danger",
      acceptLabel: "Delete",
      rejectLabel: "Cancel",
      accept: async () => {
        const id = Number(resource.id);
        const result = await deleteApiV1IdentityResourcesById({ path: { id } });
        if (!result.error) {
          toast.current?.show({
            severity: "success",
            summary: "Deleted",
            detail: `Identity resource "${resource.displayName || resource.name}" removed.`,
            life: 3000,
          });
          await load();
        }
      },
    });
  };

  const newResourceButton = (
    <Link to="new">
      <Button label="New identity resource" icon={<FontAwesomeIcon icon={faPlus} />} />
    </Link>
  );

  const showEmptyState = !loading && resources.length === 0;

  return (
    <div>
      <Toast ref={toast} position="top-right" />
      <ConfirmDialog />
      <PageHeader
        icon={faShield}
        eyebrow="Configuration"
        title="Identity Resources"
        subtitle="User claim groups returned in id tokens"
        actions={newResourceButton}
      />

      {showEmptyState ? (
        <EmptyState
          icon={faShield}
          title="No identity resources yet"
          description="Define claim groups (e.g. profile, email) that clients can request via scopes."
          action={newResourceButton}
        />
      ) : (
        <div className="overflow-hidden rounded-xl border border-border bg-surface shadow-sm">
          <div className="flex flex-wrap items-center justify-between gap-3 border-b border-border px-4 py-3">
            <span className="text-sm text-content-muted">
              {filtered.length} of {resources.length} {resources.length === 1 ? "resource" : "resources"}
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
            emptyMessage="No identity resources match your search."
            onRowClick={(e) => navigate(`${asId((e.data as IdentityResourceSummaryDto).id)}`)}
            rowClassName={() => "cursor-pointer"}
          >
            <Column
              field="name"
              header="Name"
              sortable
              body={(row: IdentityResourceSummaryDto) => (
                <span className="font-mono text-[0.85rem] text-content">{row.name}</span>
              )}
            />
            <Column
              field="displayName"
              header="Display name"
              sortable
              body={(row: IdentityResourceSummaryDto) =>
                row.displayName ?? <span className="text-content-subtle">—</span>
              }
            />
            <Column
              field="enabled"
              header="Status"
              sortable
              body={(row: IdentityResourceSummaryDto) =>
                row.enabled ? (
                  <Tag severity="success" value="Enabled" />
                ) : (
                  <Tag severity="danger" value="Disabled" />
                )
              }
            />
            <Column
              field="created"
              header="Created"
              sortable
              body={(row: IdentityResourceSummaryDto) => (
                <span className="text-sm text-content-muted">{formatDate(row.created)}</span>
              )}
            />
            <Column
              header=""
              body={(row: IdentityResourceSummaryDto) => (
                <div className="flex justify-end gap-1">
                  <Link to={asId(row.id)} onClick={(e) => e.stopPropagation()}>
                    <Button
                      text
                      rounded
                      size="small"
                      aria-label="Edit"
                      icon={<FontAwesomeIcon icon={faPencil} />}
                    />
                  </Link>
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
