import { Route, Routes } from "react-router-dom";
import RolesList from "./roles/RolesList";
import RoleEdit from "./roles/RoleEdit";

export default function Roles() {
  return (
    <Routes>
      <Route index element={<RolesList />} />
      <Route path="new" element={<RoleEdit />} />
      <Route path=":id/*" element={<RoleEdit />} />
    </Routes>
  );
}
