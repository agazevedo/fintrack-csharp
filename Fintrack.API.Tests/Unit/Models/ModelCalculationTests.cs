using Fintrack.API.Models;

namespace Fintrack.API.Tests.Unit.Models;

public class ModelCalculationTests {
	[Fact]
	public void Expense_Total_IsQuantityTimesUnitValue() {
		var expense = new Expense { Quantity = 3, UnitValue = 12.50m };

		Assert.Equal(37.50m, expense.Total);
	}

	[Fact]
	public void BudgetItem_CalculatesBudgetSpentAndRemaining() {
		var item = new BudgetItem {
			UnitValue = 100m,
			Quantity = 2,
			Expenses = {
				new Expense { Quantity = 1, UnitValue = 50m },
				new Expense { Quantity = 2, UnitValue = 30m }
			}
		};

		Assert.Equal(200m, item.BudgetTotal);
		Assert.Equal(110m, item.Spent);
		Assert.Equal(90m, item.Remaining);
	}

	[Fact]
	public void BudgetItem_WithoutExpenses_RemainingEqualsBudget() {
		var item = new BudgetItem { UnitValue = 10m, Quantity = 5 };

		Assert.Equal(0m, item.Spent);
		Assert.Equal(50m, item.Remaining);
	}
}
