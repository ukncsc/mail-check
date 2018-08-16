import { createAction, handleActions } from 'redux-actions';
import { mailCheckApiFetch } from 'common/helpers';

const fetchRequestDomainSecuritySubdomainsAction =
  'mailCheck/domainSecurity/subdomains/FETCH_REQUEST';

const fetchSuccessDomainSecuritySubdomainsAction =
  'mailCheck/domainSecurity/subdomains/FETCH_SUCCESS';

const fetchErrorDomainSecuritySubdomainsAction =
  'mailCheck/domainSecurity/subdomains/FETCH_ERROR';

export const fetchRequestDomainSecuritySubdomains = createAction(
  fetchRequestDomainSecuritySubdomainsAction
);
export const fetchSuccessDomainSecuritySubdomains = createAction(
  fetchSuccessDomainSecuritySubdomainsAction
);
export const fetchErrorDomainSecuritySubdomains = createAction(
  fetchErrorDomainSecuritySubdomainsAction
);

const transformResponse = ({ domainSecurityInfos }) =>
  domainSecurityInfos.map(({ domain: { id, name: domainName }, ...rest }) => ({
    id,
    domainName,
    ...rest,
  }));

export const fetchDomainSecuritySubdomains = (
  domainName,
  page = 1,
  pageSize = 5
) => async dispatch => {
  try {
    dispatch(
      fetchRequestDomainSecuritySubdomains({ domainName, page, pageSize })
    );
    const response = await mailCheckApiFetch(
      `/domainstatus/subdomains?search=${domainName}&page=${page}&pageSize=${pageSize}`
    );
    dispatch(
      fetchSuccessDomainSecuritySubdomains({
        domainName,
        subdomains: transformResponse(response),
      })
    );
  } catch ({ message, stack }) {
    dispatch(
      fetchErrorDomainSecuritySubdomains({
        domainName,
        error: { message, stack },
      })
    );
  }
};

const initialState = {};

export default handleActions(
  {
    [fetchRequestDomainSecuritySubdomainsAction]: (state, action) => ({
      ...state,
      [action.payload.domainName]: {
        ...state[action.payload.domainName],
        ...action.payload,
        error: null,
        loading: true,
      },
    }),

    [fetchSuccessDomainSecuritySubdomainsAction]: (state, action) => ({
      ...state,
      [action.payload.domainName]: {
        ...state[action.payload.domainName],
        ...action.payload,
        subdomains: [
          ...(state[action.payload.domainName].subdomains || []),
          ...action.payload.subdomains,
        ],
        noSubdomains:
          !action.payload.subdomains.length &&
          !(state[action.payload.domainName].subdomains || []).length,
        noMoreResults: action.payload.subdomains.length < 5,
        error: null,
        loading: false,
      },
    }),

    [fetchErrorDomainSecuritySubdomainsAction]: (state, action) => ({
      ...state,
      [action.payload.domainName]: {
        ...state[action.payload.domainName],
        ...action.payload,
        loading: false,
        noMoreResults: true,
      },
    }),
  },
  initialState
);

export const getDomainSecuritySubdomains = state => domainName =>
  state.domainSecurity.subdomains[domainName];
