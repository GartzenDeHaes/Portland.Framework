using System.Collections;
using System.Collections.Generic;

using NUnit.Framework;

namespace Portland.Collections;

[TestFixture]
public class TestIListTInsertIntoSortedListExtensions
{
	[TestCase('b', new[] { 'b', 'c', 'd', 'h', 'i' }, Description = "Contiguous Beginning")]
	[TestCase('a', new[] { 'a', 'c', 'd', 'h', 'i' }, Description = "Before Contiguous Beginning")]
	[TestCase('j', new[] { 'c', 'd', 'h', 'i', 'j' }, Description = "Contiguous End")]
	[TestCase('k', new[] { 'c', 'd', 'h', 'i', 'k' }, Description = "Past Contiguous End")]
	[TestCase('d', new[] { 'c', 'd', 'd', 'h', 'i' }, Description = "Existing element in middle")]
	[TestCase('e', new[] { 'c', 'd', 'e', 'h', 'i' }, Description = "New element in middle")]
	[TestCase('f', new[] { 'c', 'd', 'f', 'h', 'i' }, Description = "New element in middle")]
	[TestCase('g', new[] { 'c', 'd', 'g', 'h', 'i' }, Description = "New element in middle")]
	public void RespectsIComparableOrder(char value, char[] expected)
	{
		var list = new List<char>() { 'c', 'd', 'h', 'i' };
		list.InsertIntoSortedList(value);
		CollectionAssert.AreEqual(expected, list);
	}

	[Test]
	public void RespectsComparisonOrder()
	{
		var list = new List<int>() { 10, 12, 11, 13 };
		list.InsertIntoSortedList(14, SortEvenBeforeOddThenAscending);
		CollectionAssert.AreEqual(new[] { 10, 12, 14, 11, 13 }, list);
	}

	private int SortEvenBeforeOddThenAscending(int a, int b)
	{
		var aEven = (a & 1) == 0;
		var bEven = (b & 1) == 0;
		if (aEven == true && bEven == false)
		{
			return -1;
		}
		if (bEven == true && aEven == false)
		{
			return 1;
		}
		return a - b;
	}
}

[TestFixture]
public class TestIListInsertIntoSortedListExtensions
{
	[TestCase('b', new[] { 'b', 'c', 'd', 'h', 'i' }, Description = "Contiguous Beginning")]
	[TestCase('a', new[] { 'a', 'c', 'd', 'h', 'i' }, Description = "Before Contiguous Beginning")]
	[TestCase('j', new[] { 'c', 'd', 'h', 'i', 'j' }, Description = "Contiguous End")]
	[TestCase('k', new[] { 'c', 'd', 'h', 'i', 'k' }, Description = "Past Contiguous End")]
	[TestCase('d', new[] { 'c', 'd', 'd', 'h', 'i' }, Description = "Existing element in middle")]
	[TestCase('e', new[] { 'c', 'd', 'e', 'h', 'i' }, Description = "New element in middle")]
	[TestCase('f', new[] { 'c', 'd', 'f', 'h', 'i' }, Description = "New element in middle")]
	[TestCase('g', new[] { 'c', 'd', 'g', 'h', 'i' }, Description = "New element in middle")]
	public void RespectsIComparableOrder(char value, char[] expected)
	{
		var list = new ArrayList() { 'c', 'd', 'h', 'i' };
		list.InsertIntoSortedList(value);
		CollectionAssert.AreEqual(expected, list);
	}

	[Test]
	public void RespectsComparisonOrder()
	{
		var list = new ArrayList() { 10, 12, 11, 13 };
		list.InsertIntoSortedList(14, SortEvenBeforeOddThenAscending);
		CollectionAssert.AreEqual(new[] { 10, 12, 14, 11, 13 }, list);
	}

	private int SortEvenBeforeOddThenAscending(object a, object b)
	{
		var aInt = (int)a;
		var bInt = (int)b;
		var aEven = (aInt & 1) == 0;
		var bEven = (bInt & 1) == 0;
		if (aEven == true && bEven == false)
		{
			return -1;
		}
		if (bEven == true && aEven == false)
		{
			return 1;
		}
		return aInt - bInt;
	}
}