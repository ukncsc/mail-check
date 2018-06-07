import includes from 'lodash/includes';
import some from 'lodash/some';
import toString from 'lodash/toString';
import { createAction, handleActions } from 'redux-actions';
import { mailCheckApiFetch, createFetchAction } from 'common/helpers';

const fetchRequestCurrentUserAction = 'mailCheck/currentUser/FETCH_REQUEST';

const fetchSuccessCurrentUserAction = 'mailCheck/currentUser/FETCH_SUCCESS';

const fetchErrorCurrentUserAction = 'mailCheck/currentUser/FETCH_ERROR';

export const fetchRequestCurrentUser = createAction(
  fetchRequestCurrentUserAction
);

export const fetchSuccessCurrentUser = createAction(
  fetchSuccessCurrentUserAction
);

export const fetchErrorCurrentUser = createAction(fetchErrorCurrentUserAction);

export const fetchCurrentUser = createFetchAction(
  fetchRequestCurrentUser,
  fetchSuccessCurrentUser,
  fetchErrorCurrentUser,
  () => mailCheckApiFetch('/admin/user/current')
);

const agreedToTermsCurrentUserAction =
  'mailCheck/user/CURRENT_USER_AGREED_TO_TERMS';

export const agreedToTermsCurrentUser = createAction(
  agreedToTermsCurrentUserAction
);

const initialState = { isLoading: false };

export default handleActions(
  {
    [fetchRequestCurrentUserAction]: state => ({
      ...state,
      isLoading: true,
      error: null,
    }),

    [fetchSuccessCurrentUserAction]: (state, action) => ({
      ...state,
      ...action.payload,
      isLoading: false,
      error: null,
    }),

    [fetchErrorCurrentUserAction]: (state, action) => ({
      ...state,
      error: action.payload && action.payload.message,
      isLoading: false,
    }),

    [agreedToTermsCurrentUserAction]: state => ({
      ...state,
      agreedToTerms: true,
    }),
  },
  initialState
);

export const getCurrentUser = state => state.currentUser;

export const canViewAggregateData = state => domainId => {
  const currentUser = getCurrentUser(state);

  if (!currentUser.user || currentUser.user.roleType === 'Unauthorized') {
    return false;
  }

  if (currentUser.user.roleType === 'Admin') {
    return true;
  }

  return some(
    currentUser.domainPermissions,
    _ =>
      toString(_.domain.id) === domainId &&
      includes(_.permissions, 'ViewAggregateData')
  );
};
