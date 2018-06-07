import {
  recordErrorsReducer,
  recordsReducer,
} from './domain-security-store.helpers';

describe('domain security store helpers', () => {
  let result;

  describe('record errors reducer', () => {
    describe('when handling a warning', () => {
      beforeEach(() => {
        result = recordErrorsReducer(
          { warnings: ['oh woes'] },
          { errorType: 'Warning', message: 'whoopsie' }
        );
      });

      test('it should add the message to the warnings', () =>
        expect(result).toMatchObject({ warnings: ['oh woes', 'whoopsie'] }));
    });

    describe('when handling an error', () => {
      beforeEach(() => {
        result = recordErrorsReducer(
          { failures: ['uh oh'] },
          { errorType: 'Error', message: 'oh noes' }
        );
      });

      test('it should add the message to the failures', () =>
        expect(result).toMatchObject({ failures: ['uh oh', 'oh noes'] }));
    });

    describe('when handling neither an error nor warning', () => {
      beforeEach(() => {
        result = recordErrorsReducer({ an: 'accumulator' }, {});
      });

      test('it should return the accumulator', () =>
        expect(result).toMatchObject({ an: 'accumulator' }));
    });
  });

  describe('records reducer', () => {
    describe('when providing only a property', () => {
      beforeEach(() => {
        result = recordsReducer('foo')([], {
          foo: [{ value: 'meh' }, { value: 'feh' }],
        });
      });

      test('it should add the value of the property to the accumulator', () =>
        expect(result).toEqual([
          expect.objectContaining({
            foo: [{ value: 'meh' }, { value: 'feh' }],
          }),
        ]));

      test('it should concat the record values with a space', () =>
        expect(result).toEqual([
          expect.objectContaining({ record: 'meh feh' }),
        ]));
    });

    describe('when providing a separator', () => {
      beforeEach(() => {
        result = recordsReducer('bar', ';')([], {
          bar: [{ value: 'baz' }, { value: 'buzz' }],
        });
      });

      test('it should concat the record values with the provided separator', () =>
        expect(result).toEqual([
          expect.objectContaining({ record: 'baz;buzz' }),
        ]));
    });

    describe('when providing an "inheritedFrom" object', () => {
      beforeEach(() => {
        result = recordsReducer('foo', ';', { id: 123, name: 'foo.bar.com' })(
          [],
          { foo: [{ value: 'sp=foo', explanation: 'Meh.' }] }
        );
      });

      test('it should add the appropriate explanation to the sp field', () =>
        expect(result).toEqual([
          expect.objectContaining({
            foo: [
              {
                value: 'sp=foo',
                explanation:
                  'Meh. This is the policy currently being applied to this domain.',
              },
            ],
          }),
        ]));
    });
  });
});
