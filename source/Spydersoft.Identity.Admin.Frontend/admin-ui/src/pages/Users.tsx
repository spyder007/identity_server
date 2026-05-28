import { Route, Routes } from "react-router-dom";
import UsersList from "./users/UsersList";
import UserEdit from "./users/UserEdit";

export default function Users() {
  return (
    <Routes>
      <Route index element={<UsersList />} />
      <Route path="new" element={<UserEdit />} />
      <Route path=":id/*" element={<UserEdit />} />
    </Routes>
  );
}
