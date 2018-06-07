const initialState = {
  currentUser: {
    agreedToTerms:
      localStorage.getItem('mailCheck/userHasAgreedToTerms') === 'true',
  },
};
export default initialState;
