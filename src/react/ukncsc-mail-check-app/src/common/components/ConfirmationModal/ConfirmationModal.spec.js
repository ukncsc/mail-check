import React from 'react';
import { render } from 'react-testing-library';
import ConfirmationModal from './ConfirmationModal';

describe('ConfirmationModal', () => {
  let container;

  beforeEach(() => {
    ({ container } = render(
      <ConfirmationModal
        shouldShow
        description="please confirm"
        onCancel={jest.fn()}
        onConfirm={jest.fn()}
        onClose={jest.fn()}
      />
    ));
  });

  test('it should match the snapshot', async () =>
    expect(container).toMatchSnapshot());
});
