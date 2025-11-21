import React from "react";
import { Navigate } from "react-router-dom";

interface ProtectedRouteProps {
  children: React.ReactNode;
}

// Giả lập trạng thái đăng nhập, bạn nên thay bằng logic thực tế
const isAuthenticated = () => {
  // Ví dụ: kiểm tra token trong localStorage
  return !!localStorage.getItem("token");
};

const ProtectedRoute: React.FC<ProtectedRouteProps> = ({ children }) => {
  if (!isAuthenticated()) {
    return <Navigate to="/login" replace />;
  }
  return <>{children}</>;
};

export default ProtectedRoute;
