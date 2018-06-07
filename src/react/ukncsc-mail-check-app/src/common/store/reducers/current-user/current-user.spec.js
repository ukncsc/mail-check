import { canViewAggregateData, getCurrentUser } from './current-user';

describe('current user', () => {
  describe('when getting the current user', () => {
    test('it should return the current user', () =>
      expect(getCurrentUser({ currentUser: 'bacon' })).toEqual('bacon'));
  });

  describe('when checking if the user can view aggregate reports', () => {
    test('it should not allow unauthenticated users', () => {
      const result = canViewAggregateData({ currentUser: {} })('123');

      expect(result).toBe(false);
    });

    test('it should not allow unauthorised users', () => {
      const result = canViewAggregateData({
        currentUser: { roleType: 'Unauthorized' },
      })('123');

      expect(result).toBe(false);
    });

    test('it should always allow admins', () => {
      const result = canViewAggregateData({
        currentUser: { user: { roleType: 'Admin' } },
      })('123');

      expect(result).toBe(true);
    });

    test('it should not allow users without the correct domain permission', () => {
      const result = canViewAggregateData({
        currentUser: {
          user: { roleType: 'Standard' },
          domainPermissions: [{ domain: { id: 123 }, permissions: [] }],
        },
      })('123');

      expect(result).toBe(false);
    });

    test('it should allow users with the correct domain permission', () => {
      const result = canViewAggregateData({
        currentUser: {
          user: { roleType: 'Standard' },
          domainPermissions: [
            { domain: { id: 123 }, permissions: ['ViewAggregateData'] },
          ],
        },
      })('123');

      expect(result).toBe(true);
    });
  });
});
