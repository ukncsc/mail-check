import React from 'react';
import { render } from 'react-testing-library';
import DomainSecurityDetailsMessages from './DomainSecurityDetailsMessages';

describe('DomainSecurityDetailsMessages', () => {
  let container;

  describe('when provided with no failures, warnings or inconclusives', () => {
    beforeEach(() => {
      ({ container } = render(<DomainSecurityDetailsMessages type="DMARC" />));
    });

    test('it should match the snapshot', () =>
      expect(container).toMatchSnapshot());

    test('it should display a success message', () =>
      expect(
        container.getElementsByClassName('ui success message')
      ).toHaveLength(1));
  });

  describe('when provided with failures', () => {
    beforeEach(() => {
      ({ container } = render(
        <DomainSecurityDetailsMessages type="DMARC" failures={['oh noes']} />
      ));
    });

    test('it should match the snapshot', () =>
      expect(container).toMatchSnapshot());

    test('it should display a failure message', () =>
      expect(container.getElementsByClassName('ui error message')).toHaveLength(
        1
      ));

    test('it should not display any warnings messages', () =>
      expect(
        container.getElementsByClassName('ui warning message')
      ).toHaveLength(0));
  });

  describe('when provided with warnings', () => {
    beforeEach(() => {
      ({ container } = render(
        <DomainSecurityDetailsMessages type="DMARC" warnings={['oh woes']} />
      ));
    });

    test('it should match the snapshot', () =>
      expect(container).toMatchSnapshot());

    test('it should display a warning message', () =>
      expect(
        container.getElementsByClassName('ui warning message')
      ).toHaveLength(1));

    test('it should not display any failure messages', () =>
      expect(container.getElementsByClassName('ui error message')).toHaveLength(
        0
      ));
  });

  describe('when provided with inconclusives', () => {
    beforeEach(() => {
      ({ container } = render(
        <DomainSecurityDetailsMessages
          type="DMARC"
          inconclusives={['who knows']}
        />
      ));
    });

    test('it should match the snapshot', () =>
      expect(container).toMatchSnapshot());

    test('it should not display any warning messages', () =>
      expect(
        container.getElementsByClassName('ui warning message')
      ).toHaveLength(0));

    test('it should not display any failure messages', () =>
      expect(container.getElementsByClassName('ui error message')).toHaveLength(
        0
      ));
  });

  describe('when provided with failures, warnings and inconclusives', () => {
    beforeEach(() => {
      ({ container } = render(
        <DomainSecurityDetailsMessages
          type="DMARC"
          failures={['oh noes']}
          warnings={['oh woes']}
          inconclusives={['who knows']}
        />
      ));
    });

    test('it should match the snapshot', () =>
      expect(container).toMatchSnapshot());

    test('it should display a failure message', () =>
      expect(container.getElementsByClassName('ui error message')).toHaveLength(
        1
      ));

    test('it should display a warning message', () =>
      expect(container.getElementsByClassName('ui error message')).toHaveLength(
        1
      ));
  });

  describe('when provided with a markdown flag', () => {
    beforeEach(() => {
      ({ container } = render(
        <DomainSecurityDetailsMessages
          markdown
          type="DMARC"
          failures={['[link](https://ncsc.gov.uk)']}
        />
      ));
    });

    test('it should match the snapshot', () =>
      expect(container).toMatchSnapshot());

    test('it should render the text as markdown', () =>
      expect(container.getElementsByTagName('a')).toHaveLength(1));
  });
});
