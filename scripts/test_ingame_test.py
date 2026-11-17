"""Unit tests for the decisions in ingame_test.py: when results count as fresh, and what exit code they give.

Run with: python -m unittest discover -s scripts
"""
import datetime
import unittest

import ingame_test


def results(build="2026-11-17 12:00:00", request="load", passed=True, checks=None):
    """A results document shaped like the bridge's results.json."""
    if checks is None:
        checks = [{"name": "a", "passed": True, "message": "ok"}]
    return {"buildTime": build, "request": request, "passed": passed, "checks": checks}


class FreshnessTests(unittest.TestCase):
    """Results only count if they come from the build just made (and the request just written)."""

    def test_results_from_this_build_are_fresh(self):
        self.assertTrue(ingame_test.is_fresh(results(), "2026-11-17 12:00:00", None))

    def test_results_from_an_older_build_are_stale(self):
        self.assertFalse(ingame_test.is_fresh(results(build="2026-11-17 11:00:00"), "2026-11-17 12:00:00", None))

    def test_results_for_another_request_are_stale(self):
        self.assertFalse(ingame_test.is_fresh(results(request="load"), "2026-11-17 12:00:00", "abc"))

    def test_results_for_our_request_are_fresh(self):
        self.assertTrue(ingame_test.is_fresh(results(request="abc"), "2026-11-17 12:00:00", "abc"))

    def test_build_stamp_matches_the_bridge_format(self):
        moment = datetime.datetime(2026, 11, 17, 9, 5, 7, 900000)
        self.assertEqual("2026-11-17 09:05:07", ingame_test.build_stamp(moment.timestamp()))


class ExitCodeTests(unittest.TestCase):
    """Exit 0 only when every check passed; anything else is a failure."""

    def test_all_passed_exits_zero(self):
        self.assertEqual(ingame_test.PASSED, ingame_test.exit_code(results()))

    def test_a_failed_check_exits_one(self):
        checks = [{"name": "a", "passed": True}, {"name": "b", "passed": False}]
        self.assertEqual(ingame_test.FAILED, ingame_test.exit_code(results(passed=False, checks=checks)))

    def test_no_checks_is_a_failure(self):
        self.assertEqual(ingame_test.FAILED, ingame_test.exit_code(results(checks=[])))

    def test_summary_lists_each_check_and_the_total(self):
        checks = [{"name": "a", "passed": True, "message": "fine"}, {"name": "b", "passed": False, "message": "bad"}]
        lines = ingame_test.summary(results(checks=checks))
        self.assertEqual(["  PASS  a: fine", "  FAIL  b: bad", "1/2 checks passed (build 2026-11-17 12:00:00)"], lines)


if __name__ == "__main__":
    unittest.main()
