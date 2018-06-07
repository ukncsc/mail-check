/* eslint-disable import/prefer-default-export */
export const createFetchAction = (
  requestActionHandler,
  successActionHandler,
  errorActionHandler,
  request,
  transformResponse = r => r
) => (...requestArgs) => async dispatch => {
  try {
    dispatch(requestActionHandler());
    const response = await request(...requestArgs);
    dispatch(successActionHandler(transformResponse(response)));
  } catch (err) {
    dispatch(errorActionHandler(err));
  }
};
