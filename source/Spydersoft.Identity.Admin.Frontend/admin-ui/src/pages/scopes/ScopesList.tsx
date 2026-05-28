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
  faKey,
  faMagnifyingGlass,
  faPencil,
  faPlus,
  faTrash,
} from "@fortawesome/free-solid-svg-icons";

import PageHeader from "../../components/PageHeader";
import EmptyState from "../../components/EmptyState";
import { asId } from "../../utils/format";
import {
  deleteApiV1ScopesById,
  getApiV1Scopes,
} from "../../api/generated/sdk.gen";
import type { ScopeSummaryDto } from "../../api/generated/types.gen";

export default function ScopesList() {
  const navigate = useNavigate();
  const toast = useRef<Toast>(null);
  const [scopes, setScopes] = useState<ScopeSummaryDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [filter, setFilter] = useState("");

  const load = useCallback(async () => {
    setLoading(true);
    const result = await getApiV1Scopes();
    if (!result.error) setScopes(result.data ?? []);
    setLoading(false);
  }, []);

  useEffect(() => {
    void load();
  }, [load]);

  const filtered = useMemo(() => {
    if (!filter) return scopes;
    const f = filter.toLowerCase();
    return scopes.filter(
      (s) =>
        (s.name ?? "").toLowerCase().includes(f) ||
        (s.displayName ?? "").toLowerCase().includes(f),
    );
  }, [scopes, filter]);

  const handleDelete = (scope: ScopeSummaryDto) => {
    confirmDialog({
      message: (
        <span>
          Delete scope <strong>{scope.displayName || scope.name}</strong>? This cannot be undone.
        </span>
      ),
      header: "Confirm deletion",
      acceptClassName: "p-button-danger",
      acceptLabel: "Delete",
      rejectLabel: "Cancel",
      accept: async () => {
        const id = Number(scope.id);
        const result = await deleteApiV1ScopesById({ path: { id } });
        if (!result.error) {
          toast.current?.show({
            severity: "success",
            summary: "Deleted",
            detail: `Scope "${scope.displayName || scope.name}" removed.`,
            life: 3000,
          });
          await load();
        }
      },
    });
  };

  const newScopeButton = (
    <Link to="new">
      <Button label="New scope" icon={<FontAwesomeIcon icon={faPlus} />} />
    </Link>
  );

  const showEmptyState = !loading && scopes.length === 0;

  return (
    <div>
      <Toast ref={toast} position="top-right" />
      <ConfirmDialog />
      <PageHeader
        icon={faKey}
        eyebrow="Configuration"
        title="Scopes"
        subtitle="Permissions clients can request"
        actions={newScopeButton}
      />

      {showEmptyState ? (
        <EmptyState
          icon={faKey}
          title="No scopes yet"
          description="Define the API scopes available for clients to request."
          action={newScopeButton}
        />
      ) : (
        <div className="overflow-hidden rounded-xl border border-border bg-surface shadow-sm">
          <div className="flex flex-wrap items-center justify-between gap-3 border-b border-border px-4 py-3">
            <span className="text-sm text-content-muted">
              {filtered.length} of {scopes.length} {scopes.length === 1 ? "scope" : "scopes"}
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
            emptyMessage="No scopes match your search."
            onRowClick={(e) => navigate(`${asId((e.data as ScopeSummaryDto).id)}`)}
            rowClassName={() => "cursor-pointer"}
          >
            <Column
              field="name"
              header="Name"
              sortable
              body={(row: ScopeSummaryDto) => (
                <span className="font-mono text-[0.85rem] text-content">{row.name}</span>
              )}
            />
            <Column
              field="displayName"
              header="Display name"
              sortable
              body={(row: ScopeSummaryDto) =>
                row.displayName ?? <span className="text-content-subtle">—</span>
              }
            />
            <Column
              field="enabled"
              header="Status"
              sortable
              body={(row: ScopeSummaryDto) =>
                row.enabled ? (
                  <Tag severity="success" value="Enabled" />
                ) : (
                  <Tag severity="danger" value="Disabled" />
                )
              }
            />
            <Column
              header=""
              body={(row: ScopeSummaryDto) => (
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
