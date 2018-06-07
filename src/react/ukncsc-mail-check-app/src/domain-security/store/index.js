import { combineReducers } from 'redux';

import aggregateReport from './aggregate-report-info';
import dmarc from './domain-security-dmarc';
import domain from './domain-security-domain';
import spf from './domain-security-spf';
import tls from './domain-security-tls';

export * from './domain-security-dmarc';
export * from './domain-security-domain';
export * from './domain-security-spf';
export * from './domain-security-tls';
export * from './aggregate-report-info';

export default combineReducers({
  aggregateReport,
  dmarc,
  domain,
  spf,
  tls,
});
