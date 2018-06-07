import { createAction, handleActions } from 'redux-actions';
import { mailCheckApiFetch } from 'common/helpers';

const fetchRequestAction =
  'mailCheck/antiSpoofing/ANTI_SPOOFING_DOMAINS_FETCH_REQUEST';
const fetchSuccessAction =
  'mailCheck/antiSpoofing/ANTI_SPOOFING_DOMAINS_FETCH_SUCCESS';
const fetchErrorAction =
  'mailCheck/antiSpoofing/ANTI_SPOOFING_DOMAINS_FETCH_ERROR';
export const fetchRequestAntiSpoofingDomains = createAction(fetchRequestAction);
export const fetchSuccessAntiSpoofingDomains = createAction(fetchSuccessAction);
export const fetchErrorAntiSpoofingDomains = createAction(fetchErrorAction);
export const fetchAntiSpoofingDomains = (
  page,
  pageSize,
  search
) => async dispatch => {
  try {
    dispatch(fetchRequestAntiSpoofingDomains({ page, pageSize, search }));
    const response = await mailCheckApiFetch(
      `domainstatus/domains_security?page=${page}&pageSize=${pageSize}&search=${search}`
    );
    dispatch(fetchSuccessAntiSpoofingDomains(response));
  } catch (err) {
    dispatch(fetchErrorAntiSpoofingDomains(err));
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
      ...state,
      ...action.payload,
      isLoading: true,
    }),
    [fetchSuccessAction]: (state, action) => ({
      ...state,
      results: action.payload,
      error: null,
      isLoading: false,
    }),
    [fetchErrorAction]: (state, action) => ({
      ...state,
      error: action.payload.message,
      isLoading: false,
    }),
  },
  initialState
);
