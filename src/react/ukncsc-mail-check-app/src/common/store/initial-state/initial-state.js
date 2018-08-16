if (process.env.NODE_ENV === 'test') {
  global.window.localStorage = { getItem: () => null };
}

const initialState = {
  currentUser: {
    agreedToTerms:
      localStorage.getItem('mailCheck/userHasAgreedToTerms') === 'true',
  },
};
export default initialState;
