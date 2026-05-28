import { Route, Routes } from "react-router-dom";
import IdentityResourcesList from "./identity-resources/IdentityResourcesList";
import IdentityResourceEdit from "./identity-resources/IdentityResourceEdit";

export default function IdentityResources() {
  return (
    <Routes>
      <Route index element={<IdentityResourcesList />} />
      <Route path="new" element={<IdentityResourceEdit />} />
      <Route path=":id/*" element={<IdentityResourceEdit />} />
    </Routes>
  );
}
