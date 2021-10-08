using System.Reflection;
using System.Security.Cryptography;
using System.Reflection.Metadata.Ecma335;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace MorseCoder.tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        /*
         1. if input = null => Throws.ArgumentNullException
         2. if input = "a" => ".-"
         3. if input = "sos" => "...|---|..."
         4. if input = "Hola 123" => "....|---|.-..|.-| |.----|..---|...--"
         */

        [Test]
        public void Test_Null()
        {
            Assert.That(() => MorseCoder.Caracteres.Null_Exception(null), Throws.ArgumentNullException);
        }

        [Test]
        public void Test_a()
        {
            string sample = ".-";
            Assert.That(() => MorseCoder.Caracteres.CoderTranslate("a"), Is.EqualTo(sample));
        }

        
        [Test]
        public void Test_sos()
        {
            string sample = "...|---|...";
            Assert.That(() => MorseCoder.Caracteres.CoderTranslate("sos"), Is.EqualTo(sample));
        }

        [Test]
        public void Test_Hola()
        {
            string sample = "....|---|.-..|.-| |.----|..---|...--";
            Assert.That(() => MorseCoder.Caracteres.CoderTranslate("Hola 123"), Is.EqualTo(sample));
        }
    }
}