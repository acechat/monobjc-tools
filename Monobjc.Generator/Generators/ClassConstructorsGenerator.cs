//
// This file is part of Monobjc, a .NET/Objective-C bridge
// Copyright (C) 2007-2014 - Laurent Etiemble
//
// Monobjc is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// any later version.
//
// Monobjc is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with Monobjc.  If not, see <http://www.gnu.org/licenses/>.
//
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Monobjc.Tools.Generator.Model;
using Monobjc.Tools.Generator.Utilities;

namespace Monobjc.Tools.Generator.Generators
{
	/// <summary>
	///   Code generator for class's constructors.
	/// </summary>
	public class ClassConstructorsGenerator : ClassGenerator
	{
		/// <summary>
		///   Generates the specified entity.
		/// </summary>
		/// <param name = "entity">The entity.</param>
		public override void Generate (BaseEntity entity)
		{
			ClassEntity classEntity = (ClassEntity)entity;

			// Append License
			this.Writer.WriteLineFormat (0, License);

			// Append usings
			this.AppendStandardNamespaces ();

			// Append namespace starter
			this.Writer.WriteLineFormat (0, "namespace Monobjc.{0}", classEntity.Namespace);
			this.Writer.WriteLineFormat (0, "{{");

			// Append static condition if needed
			this.AppendStartCondition (classEntity);

			// Append class starter
			this.Writer.WriteLineFormat (1, "public partial class {0}", classEntity.Name);
			this.Writer.WriteLineFormat (1, "{{");

			// Append constructors
			IEnumerable<MethodEntity> constructors = classEntity.GetConstructors (true, true);

			// Filter out constructors to avoid duplicate
			constructors = constructors.Distinct (new MethodComparer ());

			foreach (MethodEntity methodEntity in constructors.Where(e => e.GenerateConstructor)) {
				this.MethodGenerator.GenerateConstructor (classEntity, methodEntity);
				this.Writer.WriteLine ();
			}

			// Append class ender
			this.Writer.WriteLineFormat (1, "}}");

			// Append static condition if needed
			this.AppendEndCondition (classEntity);

			// Append namespace ender
			this.Writer.WriteLineFormat (0, "}}");
		}
	}
}