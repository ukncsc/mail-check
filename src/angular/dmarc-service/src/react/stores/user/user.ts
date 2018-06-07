import { createAction, handleActions } from 'redux-actions';
import apiRequest from '../../services/mail-check-api.service';
import { mapKeys, camelCase } from 'lodash';

export const fetchRequestUser = createAction('mailCheck/fetchUserRequest');

export const fetchSuccessUser = createAction<any>('mailCheck/fetchUserSuccess');

export const fetchErrorUser = createAction<any>('mailCheck/fetchUserError');

export const fetchUser = () => async dispatch => {
  try {
    dispatch(fetchRequestUser());
    const {iat, access_token_expires, userinfo } = await apiRequest('/callback?info=json');
    dispatch(fetchSuccessUser(mapKeys({iat,access_token_expires, ...userinfo}, (value, key : string) => camelCase(key))));
  } catch (err) {
    dispatch(fetchErrorUser(err));
  }
};

export const initialState = {}

export default
  handleActions(
    {
      ['mailCheck/fetchUserRequest']: (state, action) => ({
        ...state,
        isLoading: true,
      }),

      ['mailCheck/fetchUserSuccess']: (state, action) => ({
        info: action.payload,
        isLoading: false,
      }),

      ['mailCheck/fetchUserError']: (state, action) => ({
        ...state,
        error: action.payload,
        isLoading: false,
      }),
    },
    initialState
  );