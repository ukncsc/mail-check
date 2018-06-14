import { createAction, handleActions } from 'redux-actions';
import { mailCheckApiFetch } from 'common/helpers';

const fetchRequestAction = 'mailCheck/myDomains/MY_DOMAINS_FETCH_REQUEST';
const fetchSuccessAction = 'mailCheck/myDomains/MY_DOMAINS_FETCH_SUCCESS';
const fetchErrorAction = 'mailCheck/myDomains/MY_DOMAINS_FETCH_ERROR';
export const fetchRequestMyDomains = createAction(fetchRequestAction);
export const fetchSuccessMyDomains = createAction(fetchSuccessAction);
export const fetchErrorMyDomains = createAction(fetchErrorAction);
export const fetchMyDomains = (page, pageSize, search) => async dispatch => {
  try {
    dispatch(fetchRequestMyDomains({ page, pageSize, search }));
    const response = await mailCheckApiFetch(
      `domainstatus/domains_security/user?page=${page}&pageSize=${pageSize}&search=${search}`
    );
    dispatch(fetchSuccessMyDomains({ response, page, pageSize }));
  } catch (err) {
    dispatch(fetchErrorMyDomains(err.message));
  }
};
const initialState = {
  page: 1,
  pageSize: 10,
  search: '',
  results: {
    domainCount: 0,
    domainSecurityInfos: [],
  },
};
export default handleActions(
  {
    [fetchRequestAction]: (state, action) => ({
      ...action.payload,
      page: state.page,
      pageSize: state.pageSize,
      isLoading: true,
    }),
    [fetchSuccessAction]: (state, action) => ({
      ...state,
      results: action.payload.response,
      page: action.payload.page,
      pageSize: action.payload.pageSize,
      error: null,
      isLoading: false,
    }),
    [fetchErrorAction]: (state, action) => ({
      page: state.page,
      pageSize: state.pageSize,
      error: action.payload,
      isLoading: false,
    }),
  },
  initialState
);
