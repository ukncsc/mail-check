export const createResourceInitialState = () => ({ isLoading: false });
export const resourceRequestReducer = state => ({
  ...state,
  isLoading: true,
});
export const resourceSuccessReducer = payloadProperty => (state, action) => ({
  ...state,
  [payloadProperty]: action.payload,
  isLoading: false,
});
export const resourceErrorReducer = (state, action) => ({
  ...state,
  error: action.payload.message,
  isLoading: false,
});
export const createResourceReducers = (
  fetchRequestAction,
  fetchSuccessAction,
  fetchErrorAction,
  payloadProperty
) => ({
  [fetchRequestAction]: resourceRequestReducer,
  [fetchSuccessAction]: resourceSuccessReducer(payloadProperty),
  [fetchErrorAction]: resourceErrorReducer,
});
