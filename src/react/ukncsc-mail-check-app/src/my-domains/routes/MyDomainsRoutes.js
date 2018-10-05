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
} from 'domain-security/containers';
import { MyDomains } from 'my-domains/containers';
import { DomainSecurityLocationContext } from 'domain-security/context';

export default () => (
  <Switch>
    <Route
      exact
      path="/my-domains/:domainId"
      component={props => (
        <DomainSecurityLocationContext.Provider value="my-domains">
          <DomainSecuritySummaryContainer {...props} />
        </DomainSecurityLocationContext.Provider>
      )}
    />
    <Route
      exact
      path="/my-domains/:domainId/dmarc"
      component={props => (
        <DomainSecurityLocationContext.Provider value="my-domains">
          <DomainSecurityDetailsDmarc {...props} />
        </DomainSecurityLocationContext.Provider>
      )}
    />
    <Route
      exact
      path="/my-domains/:domainId/spf"
      component={props => (
        <DomainSecurityLocationContext.Provider value="my-domains">
          <DomainSecurityDetailsSpf {...props} />
        </DomainSecurityLocationContext.Provider>
      )}
    />
    <Route
      exact
      path="/my-domains/:domainId/tls/:mxId"
      component={props => (
        <DomainSecurityLocationContext.Provider value="my-domains">
          <DomainSecurityDetailsTls {...props} />
        </DomainSecurityLocationContext.Provider>
      )}
    />
    <Route
      exact
      path="/my-domains/:domainId/dkim/:hostname"
      component={props => (
        <DomainSecurityLocationContext.Provider value="my-domains">
          <DomainSecurityDetailsDkimContainer {...props} />
        </DomainSecurityLocationContext.Provider>
      )}
    />
    <Route
      exact
      path="/my-domains/:domainId/tls-certificates/:hostname"
      component={props => (
        <DomainSecurityLocationContext.Provider value="my-domains">
          <DomainSecurityDetailsCertificates {...props} />
        </DomainSecurityLocationContext.Provider>
      )}
    />
    <Route
      exact
      path="/my-domains/:domainId/dmarc/aggregate-export"
      component={props => (
        <DomainSecurityLocationContext.Provider value="my-domains">
          <DomainSecurityAggregateExport {...props} />
        </DomainSecurityLocationContext.Provider>
      )}
    />
    <Route
      exact
      path="/my-domains"
      component={props => (
        <DomainSecurityLocationContext.Provider value="my-domains">
          <MyDomains {...props} />
        </DomainSecurityLocationContext.Provider>
      )}
    />
  </Switch>
);
