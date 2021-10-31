using System;
using DataCommunication;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Unit_Tests
{
	[TestClass]
	public class ByteDataTest
	{
		[TestMethod]
		public void CorrectChecksum()
		{
			if (ByteData.TryParse(out var bd0, new byte[] { 0, 0, 0, 0, 0, 0 })) Assert.AreEqual(0, bd0.Segments[0].ToByteArray()[^1], "checksum must be 0, but is not");
			else Assert.Fail("bytedata was not parsed");
			if (ByteData.TryParse(out var bd1, new byte[] { 0, 0, 1, 0, 0, 1 })) Assert.AreEqual(1, bd1.Segments[0].ToByteArray()[^1], "checksum must be 1, but is not");
			else Assert.Fail("bytedata was not parsed");

			CollectionAssert.AreEqual(new byte[] { 0, 7, 0, (byte) 'a', 0, 0, 'a' ^ 7 }, new ByteData((0, "a")).Segments[0].ToByteArray(), "byte array was not correct");
		}

		[TestMethod]
		public void ThrowsExceptions()
		{
			Assert.ThrowsException<ArgumentNullException>(() => new ByteData((0, null)));

			Assert.ThrowsException<Exception>(() => new ByteData(new byte[] { 0, 0, 0, 0, 0, 1 }));
		}
	}
}
