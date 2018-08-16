import React from 'react';
import { Route, Switch } from 'react-router-dom';
import {
  DomainSecurityAggregateExport,
  DomainSecurityDetailsCertificates,
  DomainSecurityDetailsDkimContainer,
  DomainSecurityDetailsDmarc,
  DomainSecurityDetailsSpf,
  DomainSecurityDetailsTls,
  DomainSecuritySummaryContainer,
  DomainSecurityTlsAdvice,
} from 'domain-security/containers';

export default () => (
  <Switch>
    <Route
      exact
      path="/domain-security/tls-advice"
      component={DomainSecurityTlsAdvice}
    />
    <Route
      exact
      path="/domain-security/:domainId"
      component={DomainSecuritySummaryContainer}
    />
    <Route
      exact
      path="/domain-security/:domainId/dmarc"
      component={DomainSecurityDetailsDmarc}
    />
    <Route
      exact
      path="/domain-security/:domainId/spf"
      component={DomainSecurityDetailsSpf}
    />
    <Route
      exact
      path="/domain-security/:domainId/dkim/:hostname"
      component={DomainSecurityDetailsDkimContainer}
    />
    <Route
      exact
      path="/domain-security/:domainId/tls/:mxId"
      component={DomainSecurityDetailsTls}
    />
    <Route
      exact
      path="/domain-security/:domainId/tls-certificates/:hostname"
      component={DomainSecurityDetailsCertificates}
    />
    <Route
      exact
      path="/domain-security/:domainId/dmarc/aggregate-export"
      component={DomainSecurityAggregateExport}
    />
  </Switch>
);
