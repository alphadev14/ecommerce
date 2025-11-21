import React, { useState } from "react";
import { Link } from "react-router-dom";
import { Form, Input, Button, Typography, Alert } from "antd";
import "./Login.css";
import type { LoginModel } from "../../models/Auths/Auth";
import { AuthApi } from "../../api/AuthApi";
import axios from "axios";
import "./Login.css";
import { logoFacebook, logoGoogle } from "../../images";

const { Title, Text } = Typography;

const Login: React.FC = () => {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");

  const onFinish = async (values: LoginModel) => {
    setLoading(true);
    console.log("Received values of form: ", values);
    try {
      const response = await AuthApi.login(values);
      setSuccess("Đăng nhập thành công!");
      setError("");
      console.log("reponse in line 22: ", response);

      // Lưu token vào localStorage
      localStorage.setItem("token", response.data.accessToken);
      localStorage.setItem("refreshToken", response.data.refreshToken);

      // Redirect về trang home
      window.location.href = "/";
    } catch (err: unknown) {
      if (axios.isAxiosError(err)) {
        setError(err.response?.data?.message ?? "Có lỗi xảy ra");
      } else {
        setError("Lỗi không xác định");
      }
      setSuccess("");
    }
    setLoading(false);
  };

  const onLoginWithGoogle = () => {
    const backendUrl =
      "https://localhost:7009/api/v1/google/Login?returnUrl=http://localhost:5173";
    window.location.href = backendUrl;
  };
  return (
    <div className="login-wrapper">
      <Form className="login-form" layout="vertical" onFinish={onFinish}>
        <Title level={2} className="login-title">
          Đăng nhập
        </Title>
        <Text className="login-desc">Đăng nhập vào tài khoản của bạn</Text>
        <Form.Item
          label="Tăn đăng nhập hoặc email"
          name="username"
          rules={[
            {
              required: true,
              message: "Vui lòng nhập tên đăng nhập hoặc email!",
            },
          ]}
        >
          <Input placeholder="Tên đăng nhập hoặc email" type="username" />
        </Form.Item>
        <Form.Item
          label="Mật khẩu"
          name="password"
          rules={[{ required: true, message: "Vui lòng nhập mật khẩu!" }]}
        >
          <Input.Password placeholder="Mật khẩu" />
        </Form.Item>
        {error && (
          <Alert
            type="error"
            message={error}
            showIcon
            style={{ marginBottom: 8 }}
          />
        )}
        {success && (
          <Alert
            type="success"
            message={success}
            showIcon
            style={{ marginBottom: 8 }}
          />
        )}
        <Form.Item>
          <Button
            type="primary"
            color="cyan"
            variant="solid"
            htmlType="submit"
            block
            loading={loading}
            className="login-btn"
          >
            Đăng nhập
          </Button>
        </Form.Item>
        <div className="login-bottom">
          <span>Chưa có tài khoản? </span>
          <Link to="/register" className="login-link">
            Đăng ký ngay
          </Link>
        </div>
        <div className="partner-button-container">
          <Button
            onClick={onLoginWithGoogle}
            variant="solid"
            color="danger"
            className="partner-button"
          >
            <img src={logoGoogle} width={24} height={24} />
            Google
          </Button>
          <Button variant="solid" color="geekblue" className="partner-button">
            <img src={logoFacebook} width={24} height={24} />
            Facebook
          </Button>
        </div>
      </Form>
    </div>
  );
};

export default Login;
