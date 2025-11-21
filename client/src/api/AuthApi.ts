import type { LoginModel, RegisterModel } from "../models/Auths/Auth";
import AxiosClient from "./AxiosClient";

export const AuthApi = {
  login: (data: LoginModel) => AxiosClient.post("/auth/login", data),
  register: (data: RegisterModel) => AxiosClient.post("/auth/register", data),
  refreshToken: (refreshToken: string) =>
    AxiosClient.post("/auth/refresh-token", { refreshToken }),
};
