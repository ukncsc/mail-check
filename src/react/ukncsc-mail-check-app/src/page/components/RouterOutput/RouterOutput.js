import * as React from 'react';
import { Route, Switch, Redirect } from 'react-router-dom';
import { Container } from 'semantic-ui-react';
import { Admin } from 'admin/containers';
import { Metrics } from 'metrics/containers';
import { ScrollToTop, TermsAndConditions } from 'common/components';
import { AntiSpoofing } from 'anti-spoofing/containers';
import { MyDomains } from 'my-domains/containers';
import {
  DomainSecurity,
  DomainSecurityAggregateExport,
  DomainSecurityDetails,
  DomainSecurityDetailsTls,
  DomainSecuritySummary,
  DomainSecurityTlsAdvice,
} from 'domain-security/containers';
import { DomainSecurityContext } from 'domain-security/context';

import './RouterOutput.css';

export default () => (
  <ScrollToTop>
    <Container className="RouterOutput">
      <Switch>
        <Route exact path="/metrics" component={Metrics} />
        <Route
          exact
          path="/domain-security/tls-advice"
          component={DomainSecurityTlsAdvice}
        />
        <Route
          exact
          path="/domain-security/:domainId"
          component={() => <DomainSecurity render={DomainSecuritySummary} />}
        />
        <Route
          exact
          path="/domain-security/:domainId/tls/:id"
          component={() => <DomainSecurity render={DomainSecurityDetailsTls} />}
        />
        <Route
          exact
          path="/domain-security/:domainId/:type(dmarc|spf)"
          component={() => <DomainSecurity render={DomainSecurityDetails} />}
        />
        <Route
          exact
          path="/domain-security/:domainId/dmarc/aggregate-export"
          component={() => (
            <DomainSecurity
              render={props => <DomainSecurityAggregateExport {...props} />}
            />
          )}
        />
        <Route
          exact
          path="/my-domains/:domainId"
          component={() => (
            <DomainSecurityContext.Provider value="my-domains">
              <DomainSecurity render={DomainSecuritySummary} />
            </DomainSecurityContext.Provider>
          )}
        />
        <Route
          exact
          path="/my-domains/:domainId/tls/:id"
          component={() => (
            <DomainSecurityContext.Provider value="my-domains">
              <DomainSecurity render={DomainSecurityDetailsTls} />
            </DomainSecurityContext.Provider>
          )}
        />
        <Route
          exact
          path="/my-domains/:domainId/:type(dmarc|spf)"
          component={() => (
            <DomainSecurityContext.Provider value="my-domains">
              <DomainSecurity render={DomainSecurityDetails} />
            </DomainSecurityContext.Provider>
          )}
        />
        <Route
          exact
          path="/my-domains/:domainId/dmarc/aggregate-export"
          component={() => (
            <DomainSecurity
              render={props => (
                <DomainSecurityContext.Provider value="my-domains">
                  <DomainSecurityAggregateExport {...props} />
                </DomainSecurityContext.Provider>
              )}
            />
          )}
        />
        <Route
          exact
          path="/"
          component={() => <Redirect from="/" to="/domain-security" />}
        />
        <Route exact path="/domain-security" component={AntiSpoofing} />
        <Route
          exact
          path="/my-domains"
          component={props => (
            <DomainSecurityContext.Provider value="my-domains">
              <MyDomains {...props} />
            </DomainSecurityContext.Provider>
          )}
        />
        <Route exact path="/admin" component={Admin} />
        <Route exact path="/terms" component={TermsAndConditions} />
      </Switch>
    </Container>
  </ScrollToTop>
);
