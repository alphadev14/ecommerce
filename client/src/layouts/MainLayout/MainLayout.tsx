import React from "react";
import { Layout, Avatar, Typography } from "antd";
import { CheckSquareFilled, UserOutlined } from "@ant-design/icons";
import "./MainLayout.css";

const { Header, Footer, Content } = Layout;
const { Title } = Typography;

interface MainLayoutProps {
  children: React.ReactNode;
}

const MainLayout: React.FC<MainLayoutProps> = ({ children }) => {
  return (
    <Layout style={{ minHeight: "100vh", background: "#fafbfc" }}>
      <Header className="main-header">
        <div className="main-header-left">
          <CheckSquareFilled
            style={{
              color: "#2563eb",
              fontSize: 32,
              background: "#fff",
              borderRadius: 8,
              padding: 4,
              marginRight: 12,
            }}
          />
          <Title level={4} style={{ margin: 0, color: "#111" }}>
            Todo Manager
          </Title>
        </div>
        <div className="main-header-right">
          <Avatar
            icon={<UserOutlined />}
            style={{ background: "#f3f4f6", color: "#111" }}
          />
          <span className="main-header-username">phan tam</span>
        </div>
      </Header>
      <Content
        style={{ minHeight: "calc(100vh - 64px - 40px)", marginTop: 64 }}
      >
        {children}
      </Content>
      <Footer className="main-footer">
        <span>© 2025 ToDo List. Hệ thống quản lý công việc cá nhân.</span>
      </Footer>
    </Layout>
  );
};

export default MainLayout;
