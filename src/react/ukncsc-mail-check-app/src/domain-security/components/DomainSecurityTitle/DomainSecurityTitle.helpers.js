// eslint-disable-next-line
export const getTitleIconProps = (
  failures,
  warnings,
  inconclusives,
  pending
) => {
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

  return null;
};
