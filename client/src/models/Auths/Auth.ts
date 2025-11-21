export interface LoginModel {
  username: string;
  password: string;
}

export interface RegisterModel {
  username: string;
  password: string;
  email: string;
  passwordConfirm: string;
}

export interface AuthResponseModel {
  accessToken: string;
  refreshToken: string;
  success: boolean;
}
