import React from 'react';
import AdminRoutes from 'admin/routes';
import AntiSpoofingRoutes from 'anti-spoofing/routes';
import CommonRoutes from 'common/routes';
import DomainSecurityRoutes from 'domain-security/routes';
import MetricsRoutes from 'metrics/routes';
import MyDomainsRoutes from 'my-domains/routes';
import WelcomeRoutes from 'welcome/routes';

export default () => (
  <React.Fragment>
    <AdminRoutes />
    <AntiSpoofingRoutes />
    <CommonRoutes />
    <DomainSecurityRoutes />
    <MetricsRoutes />
    <MyDomainsRoutes />
    <WelcomeRoutes />
  </React.Fragment>
);
