import React from 'react';
import { render } from 'react-testing-library';
import DomainSecurityRecordExplanation from './DomainSecurityRecordExplanation';

describe('DomainSecurityRecordExplanation', () => {
  let container;

  beforeEach(() => {
    ({ container } = render(
      <DomainSecurityRecordExplanation
        title="Test"
        data={[{ value: 'foo', explanation: 'bar' }]}
      />
    ));
  });

  test('it should match the snapshot', () =>
    expect(container).toMatchSnapshot());
});
