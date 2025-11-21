import axios from "axios";
import { AuthApi } from "./AuthApi";

const AxiosClient = axios.create({
  baseURL: "http://localhost:5123/api/v1",
  headers: {
    "Content-Type": "application/json",
  },
});

// Thêm token vào header request
AxiosClient.interceptors.request.use((config) => {
  const token = localStorage.getItem("token");
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// Xử lý khi access token hết hạn
AxiosClient.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;
    console.log(originalRequest);

    // Kiểm tra lỗi 401 (Unauthorized)
    if (
      error.response?.status === 401 &&
      !originalRequest._retry &&
      !originalRequest.url.includes("/login") &&
      !originalRequest.url.includes("/refresh")
    ) {
      originalRequest._retry = true;

      const refreshToken = localStorage.getItem("refreshToken");
      if (refreshToken) {
        try {
          // Gọi API refresh token
          const res = await AuthApi.refreshToken(refreshToken);

          // Lưu lại token mới
          localStorage.setItem("token", res.data.accessToken);
          localStorage.setItem("refreshToken", res.data.refreshToken);

          // Cập nhật header Authorization mặc định
          AxiosClient.defaults.headers.common[
            "Authorization"
          ] = `Bearer ${res.data.accessToken}`;

          // Gắn lại header cho request gốc
          originalRequest.headers[
            "Authorization"
          ] = `Bearer ${res.data.accessToken}`;

          // Thực hiện lại request gốc
          return AxiosClient(originalRequest);
          // eslint-disable-next-line @typescript-eslint/no-unused-vars
        } catch (err) {
          // Refresh thất bại → clear token và điều hướng về login
          localStorage.clear();
          window.location.href = "/login";
        }
      } else {
        // Không có refresh token → logout
        localStorage.clear();
        window.location.href = "/login";
      }
    }

    return Promise.reject(error);
  }
);

export default AxiosClient;
