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
  faIdBadge,
  faMagnifyingGlass,
  faPencil,
  faPlus,
  faTrash,
} from "@fortawesome/free-solid-svg-icons";

import PageHeader from "../../components/PageHeader";
import EmptyState from "../../components/EmptyState";
import { formatDate, asId } from "../../utils/format";
import {
  deleteApiV1ClientsById,
  getApiV1Clients,
} from "../../api/generated/sdk.gen";
import type { ClientSummaryDto } from "../../api/generated/types.gen";

export default function ClientsList() {
  const navigate = useNavigate();
  const toast = useRef<Toast>(null);
  const [clients, setClients] = useState<ClientSummaryDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [filter, setFilter] = useState("");

  const load = useCallback(async () => {
    setLoading(true);
    const result = await getApiV1Clients();
    if (!result.error) setClients(result.data ?? []);
    setLoading(false);
  }, []);

  useEffect(() => {
    void load();
  }, [load]);

  const filtered = useMemo(() => {
    if (!filter) return clients;
    const f = filter.toLowerCase();
    return clients.filter(
      (c) =>
        (c.clientId ?? "").toLowerCase().includes(f) ||
        (c.clientName ?? "").toLowerCase().includes(f),
    );
  }, [clients, filter]);

  const handleDelete = (client: ClientSummaryDto) => {
    confirmDialog({
      message: (
        <span>
          Delete client <strong>{client.clientName || client.clientId}</strong>? This cannot be undone.
        </span>
      ),
      header: "Confirm deletion",
      acceptClassName: "p-button-danger",
      acceptLabel: "Delete",
      rejectLabel: "Cancel",
      accept: async () => {
        const id = Number(client.id);
        const result = await deleteApiV1ClientsById({ path: { id } });
        if (!result.error) {
          toast.current?.show({
            severity: "success",
            summary: "Deleted",
            detail: `Client "${client.clientName || client.clientId}" removed.`,
            life: 3000,
          });
          await load();
        }
      },
    });
  };

  const newClientButton = (
    <Link to="new">
      <Button label="New client" icon={<FontAwesomeIcon icon={faPlus} />} />
    </Link>
  );

  const showEmptyState = !loading && clients.length === 0;

  return (
    <div>
      <Toast ref={toast} position="top-right" />
      <ConfirmDialog />
      <PageHeader
        icon={faIdBadge}
        eyebrow="Configuration"
        title="Clients"
        subtitle="Manage OAuth 2.0 and OpenID Connect clients"
        actions={newClientButton}
      />

      {showEmptyState ? (
        <EmptyState
          icon={faIdBadge}
          title="No clients yet"
          description="Register your first OAuth 2.0 or OpenID Connect client to start issuing tokens."
          action={newClientButton}
        />
      ) : (
        <div className="overflow-hidden rounded-xl border border-border bg-surface shadow-sm">
          <div className="flex flex-wrap items-center justify-between gap-3 border-b border-border px-4 py-3">
            <span className="text-sm text-content-muted">
              {filtered.length} of {clients.length} {clients.length === 1 ? "client" : "clients"}
            </span>
            <div className="relative">
              <FontAwesomeIcon
                icon={faMagnifyingGlass}
                className="pointer-events-none absolute left-3 top-1/2 -translate-y-1/2 text-xs text-content-subtle"
              />
              <InputText
                value={filter}
                onChange={(e) => setFilter(e.target.value)}
                placeholder="Search by id or name"
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
            emptyMessage="No clients match your search."
            onRowClick={(e) => navigate(`${asId((e.data as ClientSummaryDto).id)}`)}
            rowClassName={() => "cursor-pointer"}
          >
            <Column
              field="clientId"
              header="Client ID"
              sortable
              body={(row: ClientSummaryDto) => (
                <span className="font-mono text-[0.85rem] text-content">{row.clientId}</span>
              )}
            />
            <Column
              field="clientName"
              header="Name"
              sortable
              body={(row: ClientSummaryDto) =>
                row.clientName ?? <span className="text-content-subtle">—</span>
              }
            />
            <Column
              field="protocolType"
              header="Protocol"
              sortable
              body={(row: ClientSummaryDto) => (
                <span className="text-xs uppercase tracking-wide text-content-muted">
                  {row.protocolType}
                </span>
              )}
            />
            <Column
              field="enabled"
              header="Status"
              sortable
              body={(row: ClientSummaryDto) =>
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
              body={(row: ClientSummaryDto) => (
                <span className="text-sm text-content-muted">{formatDate(row.created)}</span>
              )}
            />
            <Column
              header=""
              body={(row: ClientSummaryDto) => (
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
