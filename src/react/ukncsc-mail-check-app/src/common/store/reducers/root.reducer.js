import { combineReducers } from 'redux';
import admin from 'admin/store';
import domain from 'anti-spoofing/store/domains';
import domainSecurity from 'domain-security/store';
import metrics from 'metrics/store';
import mydomains from 'my-domains/store';
import welcome from 'welcome/store';

import currentUser from './current-user';

export default combineReducers({
  admin,
  currentUser,
  domain,
  domainSecurity,
  metrics,
  mydomains,
  welcome,
});
