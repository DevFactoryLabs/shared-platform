#!/bin/bash
set -e

COVERAGE_THRESHOLD=85

echo "==> Restoring tools..."
dotnet tool restore

echo ""
echo "==> Checking code formatting (CSharpier)..."
dotnet csharpier . --check

echo ""
echo "==> Building solution (analyzers + warnings as errors)..."
dotnet build shared-platform.slnx --no-restore --configuration Release

echo ""
echo "==> Running tests with coverage..."
rm -rf coverage-results coverage-report

dotnet test shared-platform.slnx \
  --no-build \
  --configuration Release \
  --collect:"XPlat Code Coverage" \
  --results-directory ./coverage-results

echo ""
echo "==> Generating coverage report..."
dotnet reportgenerator \
  -reports:"coverage-results/**/coverage.cobertura.xml" \
  -targetdir:"coverage-report" \
  -reporttypes:"Html;TextSummary"

echo ""
cat coverage-report/Summary.txt

echo ""

# Extract line coverage percentage and enforce threshold
LINE_COVERAGE=$(grep -oP 'Line coverage:\s+\K[\d.]+' coverage-report/Summary.txt)

if [ -z "$LINE_COVERAGE" ]; then
  echo "ERROR: Could not read line coverage from report."
  exit 1
fi

echo "Line coverage: ${LINE_COVERAGE}% (threshold: ${COVERAGE_THRESHOLD}%)"

if awk "BEGIN { exit ($LINE_COVERAGE >= $COVERAGE_THRESHOLD) }"; then
  echo "FAILED: Coverage ${LINE_COVERAGE}% is below the required ${COVERAGE_THRESHOLD}%."
  exit 1
else
  echo "OK: Coverage meets the required threshold."
fi

echo ""
echo "Full report: coverage-report/index.html"
