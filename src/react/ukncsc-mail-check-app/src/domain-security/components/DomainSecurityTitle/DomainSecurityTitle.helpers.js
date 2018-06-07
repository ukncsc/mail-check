// eslint-disable-next-line
export const getTitleIconProps = (
  error,
  failures,
  warnings,
  inconclusives,
  pending
) => {
  if (error) {
    return { name: 'close', color: 'red' };
  }

  if (failures && !!failures.length) {
    return { name: 'exclamation circle', color: 'red' };
  }

  if (warnings && !!warnings.length) {
    return { name: 'exclamation triangle', color: 'yellow' };
  }

  if (inconclusives && !!inconclusives.length) {
    return { name: 'question circle', color: 'grey' };
  }

  if (pending) {
    return { name: 'time', color: 'blue' };
  }

  return { name: 'check circle', color: 'green' };
};
