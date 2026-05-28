import { useCallback, useEffect, useState } from "react";
import { DataTable } from "primereact/datatable";
import { Column, type ColumnProps } from "primereact/column";
import { Button } from "primereact/button";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faTrash } from "@fortawesome/free-solid-svg-icons";
import { confirmDialog, ConfirmDialog } from "primereact/confirmdialog";
import type { ReactNode } from "react";

export interface SubResourceColumn<T> {
  field: keyof T & string;
  header: string;
  body?: ColumnProps["body"];
  className?: string;
}

export interface SubResourceListProps<T extends { id?: number | string }, TCreate> {
  title: string;
  emptyMessage?: string;
  load: () => Promise<T[]>;
  remove: (id: number) => Promise<void>;
  columns: SubResourceColumn<T>[];
  describe: (row: T) => string;
  /** Form children render the create form. Calls onCreated() after a successful POST to refresh the table. */
  renderCreateForm: (args: { onCreated: () => void; createDraft: TCreate; setCreateDraft: (next: TCreate) => void }) => ReactNode;
  emptyCreateDraft: TCreate;
}

export default function SubResourceList<T extends { id?: number | string }, TCreate>(
  {
    title,
    emptyMessage = "Nothing here yet.",
    load,
    remove,
    columns,
    describe,
    renderCreateForm,
    emptyCreateDraft,
  }: Readonly<SubResourceListProps<T, TCreate>>,
) {
  const [items, setItems] = useState<T[]>([]);
  const [loading, setLoading] = useState(true);
  const [createDraft, setCreateDraft] = useState<TCreate>(emptyCreateDraft);

  const refresh = useCallback(async () => {
    setLoading(true);
    try {
      setItems(await load());
    } finally {
      setLoading(false);
    }
  }, [load]);

  useEffect(() => {
    void refresh();
  }, [refresh]);

  const handleDelete = (row: T) => {
    confirmDialog({
      message: (
        <span>
          Delete <strong>{describe(row)}</strong>?
        </span>
      ),
      header: "Confirm deletion",
      acceptClassName: "p-button-danger",
      acceptLabel: "Delete",
      rejectLabel: "Cancel",
      accept: async () => {
        await remove(Number(row.id));
        await refresh();
      },
    });
  };

  return (
    <div className="overflow-hidden rounded-xl border border-border bg-surface shadow-sm">
      <ConfirmDialog />

      <header className="flex items-baseline justify-between gap-3 border-b border-border px-5 py-3.5">
        <div>
          <h3 className="text-sm font-semibold text-content">{title}</h3>
          <p className="mt-0.5 text-xs text-content-muted">{countLabel(loading, items.length)}</p>
        </div>
      </header>

      <DataTable
        value={items}
        loading={loading}
        dataKey="id"
        emptyMessage={emptyMessage}
        size="small"
      >
        {columns.map((col) => (
          <Column key={col.field} field={col.field} header={col.header} body={col.body} className={col.className} />
        ))}
        <Column
          header=""
          style={{ width: "4.5rem" }}
          body={(row: T) => (
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
          Add new
        </h4>
        {renderCreateForm({
          onCreated: () => {
            setCreateDraft(emptyCreateDraft);
            void refresh();
          },
          createDraft,
          setCreateDraft,
        })}
      </div>
    </div>
  );
}

function countLabel(loading: boolean, count: number): string {
  if (loading) return "Loading…";
  return `${count} ${count === 1 ? "entry" : "entries"}`;
}

export { asId } from "../utils/format";
