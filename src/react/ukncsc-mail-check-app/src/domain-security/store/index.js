import { combineReducers } from 'redux';

import aggregateData from './domain-security-aggregate-data';
import certificates from './domain-security-certificates';
import dmarc from './domain-security-dmarc';
import dkim from './domain-security-dkim';
import domain from './domain-security-domain';
import spf from './domain-security-spf';
import subdomains from './domain-security-subdomains';
import tls from './domain-security-tls';

export * from './domain-security-aggregate-data';
export * from './domain-security-certificates';
export * from './domain-security-dkim';
export * from './domain-security-dmarc';
export * from './domain-security-domain';
export * from './domain-security-spf';
export * from './domain-security-subdomains';
export * from './domain-security-tls';

export default combineReducers({
  aggregateData,
  certificates,
  dkim,
  dmarc,
  domain,
  spf,
  subdomains,
  tls,
});
