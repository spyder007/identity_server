import { useEffect, useRef } from "react";
import { Toast } from "primereact/toast";
import { onApiError } from "../api/client";

export default function GlobalToast() {
  const toast = useRef<Toast>(null);

  useEffect(() => {
    return onApiError((err) => {
      if (err.kind === "auth") return;
      toast.current?.show({
        severity: err.kind === "forbidden" || err.kind === "validation" ? "warn" : "error",
        summary:
          err.kind === "forbidden"
            ? "Forbidden"
            : err.kind === "notfound"
              ? "Not found"
              : err.kind === "validation"
                ? "Invalid request"
                : err.kind === "server"
                  ? "Server error"
                  : "Network error",
        detail: err.message,
        life: 4000,
      });
    });
  }, []);

  return <Toast ref={toast} position="top-right" />;
}
