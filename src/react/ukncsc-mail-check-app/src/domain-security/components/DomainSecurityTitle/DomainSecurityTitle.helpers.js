// eslint-disable-next-line
export const getTitleIconProps = (failures, warnings, inconclusives) => {
  if (failures && !!failures.length) {
    return { name: 'exclamation circle', color: 'red' };
  }

  if (warnings && !!warnings.length) {
    return { name: 'exclamation triangle', color: 'yellow' };
  }

  if (inconclusives && !!inconclusives.length) {
    return { name: 'question circle', color: 'grey' };
  }

  return null;
};
