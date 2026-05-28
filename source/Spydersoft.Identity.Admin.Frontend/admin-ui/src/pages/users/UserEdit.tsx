import { useCallback, useEffect, useRef, useState } from "react";
import { Link, useNavigate, useParams } from "react-router-dom";
import { InputText } from "primereact/inputtext";
import { InputSwitch } from "primereact/inputswitch";
import { Password } from "primereact/password";
import { Button } from "primereact/button";
import { TabView, TabPanel } from "primereact/tabview";
import { Toast } from "primereact/toast";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  faArrowLeft,
  faSave,
  faUser,
} from "@fortawesome/free-solid-svg-icons";

import PageHeader from "../../components/PageHeader";
import {
  getApiV1UsersById,
  postApiV1Users,
  putApiV1UsersById,
} from "../../api/generated/sdk.gen";
import type {
  CreateUserDto,
  SaveUserDto,
  UserDto,
} from "../../api/generated/types.gen";
import { RolesPanel, ClaimsPanel } from "./UserSubResources";

interface Draft extends SaveUserDto {
  password: string;
}

function makeEmptyDraft(): Draft {
  return {
    userName: "",
    email: "",
    name: null,
    phoneNumber: null,
    twoFactorEnabled: false,
    lockoutEnabled: true,
    password: "",
  };
}

function toDraft(dto: UserDto): Draft {
  return {
    userName: dto.userName ?? "",
    email: dto.email ?? "",
    name: dto.name ?? null,
    phoneNumber: dto.phoneNumber ?? null,
    twoFactorEnabled: dto.twoFactorEnabled ?? false,
    lockoutEnabled: dto.lockoutEnabled ?? false,
    password: "",
  };
}

export default function UserEdit() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const toast = useRef<Toast>(null);
  const isNew = !id;

  const [draft, setDraft] = useState<Draft>(makeEmptyDraft);
  const [loading, setLoading] = useState(!isNew);
  const [saving, setSaving] = useState(false);
  const [activeTab, setActiveTab] = useState(0);

  const load = useCallback(async () => {
    if (isNew || !id) return;
    setLoading(true);
    const r = await getApiV1UsersById({ path: { id } });
    if (!r.error && r.data) setDraft(toDraft(r.data));
    setLoading(false);
  }, [isNew, id]);

  useEffect(() => {
    void load();
  }, [load]);

  const update = <K extends keyof Draft>(key: K, value: Draft[K]) =>
    setDraft((d) => ({ ...d, [key]: value }));

  const submit = async (e: React.FormEvent) => {
    e.preventDefault();
    setSaving(true);
    try {
      if (isNew) {
        const body: CreateUserDto = {
          userName: draft.userName,
          email: draft.email,
          name: draft.name ?? null,
          phoneNumber: draft.phoneNumber ?? null,
          twoFactorEnabled: draft.twoFactorEnabled,
          lockoutEnabled: draft.lockoutEnabled,
          password: draft.password,
        };
        const r = await postApiV1Users({ body });
        if (!r.error && r.data?.id) {
          toast.current?.show({
            severity: "success",
            summary: "Created",
            detail: "User created.",
            life: 3000,
          });
          navigate(`/users/${r.data.id}`, { replace: true });
        }
      } else if (id) {
        const body: SaveUserDto = {
          userName: draft.userName,
          email: draft.email,
          name: draft.name ?? null,
          phoneNumber: draft.phoneNumber ?? null,
          twoFactorEnabled: draft.twoFactorEnabled,
          lockoutEnabled: draft.lockoutEnabled,
        };
        const r = await putApiV1UsersById({ path: { id }, body });
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
        icon={faUser}
        eyebrow={isNew ? "New" : "User"}
        title={isNew ? "Create user" : draft.userName || draft.email || "Edit user"}
        subtitle={isNew ? "Add a new account" : draft.email}
        actions={
          <Link to="/users">
            <Button outlined label="Back" icon={<FontAwesomeIcon icon={faArrowLeft} />} />
          </Link>
        }
      />

      <form onSubmit={submit}>
        <TabView scrollable activeIndex={activeTab} onTabChange={(e) => setActiveTab(e.index)}>
          <TabPanel header="Settings">
            <div className="space-y-8">
              <Section
                title="Account"
                description="Login identity and contact details."
              >
                <FieldGrid>
                  <Field label="Username" required>
                    <InputText
                      value={draft.userName}
                      onChange={(e) => update("userName", e.target.value)}
                      required
                      disabled={!isNew}
                      placeholder="jane.doe"
                    />
                  </Field>
                  <Field label="Email" required>
                    <InputText
                      type="email"
                      value={draft.email}
                      onChange={(e) => update("email", e.target.value)}
                      required
                      placeholder="jane@example.com"
                    />
                  </Field>
                  <Field label="Display name">
                    <InputText
                      value={draft.name ?? ""}
                      onChange={(e) => update("name", e.target.value || null)}
                      placeholder="Jane Doe"
                    />
                  </Field>
                  <Field label="Phone number">
                    <InputText
                      value={draft.phoneNumber ?? ""}
                      onChange={(e) => update("phoneNumber", e.target.value || null)}
                      placeholder="+1 555 123 4567"
                    />
                  </Field>
                  {isNew && (
                    <Field label="Password" required className="md:col-span-2">
                      <Password
                        value={draft.password}
                        onChange={(e) => update("password", e.target.value)}
                        required
                        toggleMask
                        feedback={false}
                        inputClassName="w-full"
                        className="w-full"
                      />
                    </Field>
                  )}
                </FieldGrid>
              </Section>

              <Section
                title="Security"
                description="Multi-factor and lockout behaviour."
              >
                <SwitchList>
                  <SwitchRow
                    label="Two-factor authentication"
                    description="Require a second factor when this user signs in."
                    checked={draft.twoFactorEnabled ?? false}
                    onChange={(v) => update("twoFactorEnabled", v)}
                  />
                  <SwitchRow
                    label="Lockout enabled"
                    description="Allow this account to be locked out after repeated failed sign-ins."
                    checked={draft.lockoutEnabled ?? false}
                    onChange={(v) => update("lockoutEnabled", v)}
                  />
                </SwitchList>
              </Section>
            </div>
          </TabPanel>

          {!isNew && id && <TabPanel header="Roles"><RolesPanel userId={id} /></TabPanel>}
          {!isNew && id && <TabPanel header="Claims"><ClaimsPanel userId={id} /></TabPanel>}
        </TabView>

        <div className="fixed inset-x-0 bottom-0 left-60 z-10 border-t border-border bg-surface/95 backdrop-blur">
          <div className="mx-auto flex w-full max-w-6xl items-center justify-end gap-2 px-6 py-3 md:px-8">
            <Link to="/users">
              <Button type="button" outlined label="Cancel" />
            </Link>
            <Button
              type="submit"
              loading={saving}
              label={isNew ? "Create user" : "Save changes"}
              icon={<FontAwesomeIcon icon={faSave} />}
            />
          </div>
        </div>
      </form>
    </div>
  );
}

function Section({
  title,
  description,
  children,
}: Readonly<{
  title: string;
  description?: string;
  children: React.ReactNode;
}>) {
  return (
    <section className="rounded-xl border border-border bg-surface p-5 shadow-sm md:p-6">
      <header className="mb-5">
        <h2 className="text-base font-semibold text-content">{title}</h2>
        {description && (
          <p className="mt-0.5 text-sm text-content-muted">{description}</p>
        )}
      </header>
      <div className="space-y-5">{children}</div>
    </section>
  );
}

function FieldGrid({ children }: Readonly<{ children: React.ReactNode }>) {
  return <div className="grid grid-cols-1 gap-4 md:grid-cols-2">{children}</div>;
}

function SwitchList({ children }: Readonly<{ children: React.ReactNode }>) {
  return (
    <div className="divide-y divide-border rounded-md border border-border">
      {children}
    </div>
  );
}

function Field({
  label,
  required,
  className,
  children,
}: Readonly<{
  label: string;
  required?: boolean;
  className?: string;
  children: React.ReactNode;
}>) {
  return (
    <div className={className ?? ""}>
      <label className="mb-1.5 block text-sm font-medium text-content">
        {label}
        {required && <span className="ml-1 text-danger">*</span>}
      </label>
      <div className="w-full [&_.p-inputtext]:w-full [&_.p-inputtextarea]:w-full [&_.p-inputnumber]:w-full">
        {children}
      </div>
    </div>
  );
}

function SwitchRow({
  label,
  description,
  checked,
  onChange,
}: Readonly<{
  label: string;
  description?: string;
  checked: boolean;
  onChange: (next: boolean) => void;
}>) {
  return (
    <label className="flex cursor-pointer items-center justify-between gap-4 px-4 py-3 transition-colors hover:bg-surface-muted">
      <span className="flex min-w-0 flex-1 items-start gap-2">
        <span className="min-w-0">
          <span className="block text-sm font-medium text-content">{label}</span>
          {description && (
            <span className="mt-0.5 block text-xs text-content-muted">{description}</span>
          )}
        </span>
      </span>
      <InputSwitch checked={checked} onChange={(e) => onChange(Boolean(e.value))} />
    </label>
  );
}
