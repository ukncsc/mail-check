import { createAction, handleActions } from 'redux-actions';
import reduce from 'lodash/reduce';
import { mailCheckApiFetch } from 'common/helpers';
import { recordErrorsReducer } from './helpers';

const fetchRequestDomainSecurityCertificatesAction =
  'mailCheck/domainSecurity/certificates/FETCH_REQUEST';

const fetchSuccessDomainSecurityCertificatesAction =
  'mailCheck/domainSecurity/certificates/FETCH_SUCCESS';

const fetchErrorDomainSecurityCertificatesAction =
  'mailCheck/domainSecurity/certificates/FETCH_ERROR';

export const fetchRequestDomainSecurityCertificates = createAction(
  fetchRequestDomainSecurityCertificatesAction
);
export const fetchSuccessDomainSecurityCertificates = createAction(
  fetchSuccessDomainSecurityCertificatesAction
);
export const fetchErrorDomainSecurityCertificates = createAction(
  fetchErrorDomainSecurityCertificatesAction
);

export const fetchDomainSecurityCertificates = domainName => async dispatch => {
  try {
    dispatch(fetchRequestDomainSecurityCertificates({ domainName }));
    const response = await mailCheckApiFetch(
      `/certificates/domain/${domainName}`
    );
    dispatch(
      fetchSuccessDomainSecurityCertificates({ domainName, ...response })
    );
  } catch ({ message, stack }) {
    dispatch(
      fetchErrorDomainSecurityCertificates({ domainName, message, stack })
    );
  }
};

const initialState = {};

const transformResponse = ({ domainName, hostResults }) =>
  !hostResults
    ? { pending: true }
    : {
        domainName,
        records: hostResults
          .filter(({ hostName }) => Boolean(hostName))
          .map(
            ({ hostName: hostname, certificates: certs, errors, ...rest }) => ({
              hostname,
              certs,
              ...rest,
              ...reduce(errors, recordErrorsReducer, {
                warnings: [],
                failures: [],
                inconclusives: [],
              }),
            })
          ),
      };

export default handleActions(
  {
    [fetchRequestDomainSecurityCertificatesAction]: (state, action) => ({
      ...state,
      [action.payload.domainName]: { loading: true },
    }),

    [fetchSuccessDomainSecurityCertificatesAction]: (state, action) => ({
      ...state,
      [action.payload.domainName]: transformResponse(action.payload),
    }),

    [fetchErrorDomainSecurityCertificatesAction]: (state, action) => ({
      ...state,
      [action.payload.domainName]: { error: action.payload },
    }),
  },
  initialState
);

export const getDomainSecurityCertificates = state => domainName =>
  state.domainSecurity.certificates[domainName];
