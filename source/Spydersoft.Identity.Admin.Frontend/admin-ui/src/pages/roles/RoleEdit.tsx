import { useCallback, useEffect, useRef, useState } from "react";
import { Link, useNavigate, useParams } from "react-router-dom";
import { InputText } from "primereact/inputtext";
import { Button } from "primereact/button";
import { TabView, TabPanel } from "primereact/tabview";
import { Toast } from "primereact/toast";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  faArrowLeft,
  faSave,
  faUserShield,
} from "@fortawesome/free-solid-svg-icons";

import PageHeader from "../../components/PageHeader";
import {
  getApiV1RolesById,
  postApiV1Roles,
  putApiV1RolesById,
} from "../../api/generated/sdk.gen";
import type { RoleDto, SaveRoleDto } from "../../api/generated/types.gen";
import { ClaimsPanel } from "./RoleSubResources";

function makeEmptyDraft(): SaveRoleDto {
  return { name: "" };
}

function toDraft(dto: RoleDto): SaveRoleDto {
  return { name: dto.name ?? "" };
}

export default function RoleEdit() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const toast = useRef<Toast>(null);
  const isNew = !id;

  const [draft, setDraft] = useState<SaveRoleDto>(makeEmptyDraft);
  const [loading, setLoading] = useState(!isNew);
  const [saving, setSaving] = useState(false);
  const [activeTab, setActiveTab] = useState(0);

  const load = useCallback(async () => {
    if (isNew || !id) return;
    setLoading(true);
    const r = await getApiV1RolesById({ path: { id } });
    if (!r.error && r.data) setDraft(toDraft(r.data));
    setLoading(false);
  }, [isNew, id]);

  useEffect(() => {
    void load();
  }, [load]);

  const submit = async () => {
    setSaving(true);
    try {
      if (isNew) {
        const r = await postApiV1Roles({ body: draft });
        if (!r.error && r.data?.id) {
          toast.current?.show({
            severity: "success",
            summary: "Created",
            detail: "Role created.",
            life: 3000,
          });
          navigate(`/roles/${r.data.id}`, { replace: true });
        }
      } else if (id) {
        const r = await putApiV1RolesById({ path: { id }, body: draft });
        if (!r.error) {
          toast.current?.show({
            severity: "success",
            summary: "Saved",
            detail: "Changes saved.",
            life: 3000,
          });
        }
      }
    } finally {
      setSaving(false);
    }
  };

  if (loading) {
    return <div className="p-4 text-content-muted">Loading…</div>;
  }

  return (
    <div className="pb-24">
      <Toast ref={toast} position="top-right" />
      <PageHeader
        icon={faUserShield}
        eyebrow={isNew ? "New" : "Role"}
        title={isNew ? "Create role" : draft.name || "Edit role"}
        subtitle={isNew ? "Define a role users can be assigned to" : undefined}
        actions={
          <Link to="/roles">
            <Button outlined label="Back" icon={<FontAwesomeIcon icon={faArrowLeft} />} />
          </Link>
        }
      />

      <TabView scrollable activeIndex={activeTab} onTabChange={(e) => setActiveTab(e.index)}>
          <TabPanel header="Settings">
            <section className="rounded-xl border border-border bg-surface p-5 shadow-sm md:p-6">
              <header className="mb-5">
                <h2 className="text-base font-semibold text-content">Basic information</h2>
                <p className="mt-0.5 text-sm text-content-muted">
                  Roles are referenced by name when assigning to users.
                </p>
              </header>
              <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
                <div>
                  <label className="mb-1.5 block text-sm font-medium text-content">
                    Name
                    <span className="ml-1 text-danger">*</span>
                  </label>
                  <div className="w-full [&_.p-inputtext]:w-full">
                    <InputText
                      value={draft.name}
                      onChange={(e) => setDraft({ name: e.target.value })}
                      required
                      placeholder="administrator"
                    />
                  </div>
                </div>
              </div>
            </section>
          </TabPanel>

          {!isNew && id && <TabPanel header="Claims"><ClaimsPanel roleId={id} /></TabPanel>}
        </TabView>

        <div className="fixed inset-x-0 bottom-0 left-60 z-10 border-t border-border bg-surface/95 backdrop-blur">
          <div className="mx-auto flex w-full max-w-6xl items-center justify-end gap-2 px-6 py-3 md:px-8">
            <Link to="/roles">
              <Button type="button" outlined label="Cancel" />
            </Link>
            <Button
              type="button"
              onClick={submit}
              loading={saving}
              label={isNew ? "Create role" : "Save changes"}
              icon={<FontAwesomeIcon icon={faSave} />}
            />
          </div>
        </div>
    </div>
  );
}
