﻿//
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
using System;
using System.Linq;
using Monobjc.Tools.Generator.Model;
using Monobjc.Tools.Generator.Utilities;

namespace Monobjc.Tools.Generator.Generators
{
	/// <summary>
	///   Base class for typed entity that generate wrapper for opaque functions.
	/// </summary>
	public class OpaqueTypedGenerator : BaseGenerator
	{
		private const String OPAQUE_POINTER = "__opaque";

		private FunctionGenerator functionGenerator;

		/// <summary>
		///   Gets or sets the function generator.
		/// </summary>
		/// <value>The function generator.</value>
		protected FunctionGenerator FunctionGenerator {
			get {
				if (this.functionGenerator == null) {
					this.functionGenerator = new FunctionGenerator () {
						Writer = this.Writer,
						Statistics = this.Statistics,
						MixedTypes = this.MixedTypes,
						TypeManager = this.TypeManager,
					};
				}
				return this.functionGenerator;
			}
		}
		
		public override void Generate (BaseEntity entity)
		{
			TypedEntity typedEntity = (TypedEntity)entity;

			// Append License
			this.Writer.WriteLineFormat (0, License);

			// Append usings
			this.AppendStandardNamespaces ();

			// Append namespace starter
			this.Writer.WriteLineFormat (0, "namespace Monobjc.{0}", typedEntity.Namespace);
			this.Writer.WriteLineFormat (0, "{{");

			// Append static condition if needed
			this.AppendStartCondition (typedEntity);

			// Append class starter
#if GENERATED_ATTRIBUTE
                //this.Writer.WriteLineFormat(1, "[GeneratedCodeAttribute(\"{0}\", \"{1}\")]", this.ToolName, this.ToolVersion);
#endif
			String baseType = typedEntity.BaseType ?? String.Empty;
			if (String.IsNullOrEmpty(baseType)) {
				this.Writer.WriteLineFormat (1, "public partial class {0}", typedEntity.Name);
			} else {
				this.Writer.WriteLineFormat (1, "public partial class {0} : {1}", typedEntity.Name, baseType);
			}
			this.Writer.WriteLineFormat (1, "{{");

			// Add a pointer if needed
			if (String.IsNullOrEmpty(baseType)) {
				this.Writer.WriteLineFormat (2, "protected IntPtr {0};", OPAQUE_POINTER);
			}

			// Output the wrapping code
			this.Writer.WriteLineFormat (2, "/// <summary>");
			this.Writer.WriteLineFormat (2, "/// Initializes a new instance of the <see cref=\"{0}\"/> class.", typedEntity.Name);
			this.Writer.WriteLineFormat (2, "/// </summary>");
			this.Writer.WriteLineFormat (2, "/// <param name=\"value\">The native pointer.</param>");
			if (String.IsNullOrEmpty(baseType)) {
				this.Writer.WriteLineFormat (2, "public {0}(IntPtr value)", typedEntity.Name);
				this.Writer.WriteLineFormat (2, "{{");
				this.Writer.WriteLineFormat (3, "this.{0} = value;", OPAQUE_POINTER);
				this.Writer.WriteLineFormat (2, "}}");
			} else {
				this.Writer.WriteLineFormat (2, "public {0}(IntPtr value) : base(value) {{}}", typedEntity.Name);
			}
			this.Writer.WriteLine ();

			// Output the opaque functions
			foreach (FunctionEntity functionEntity in typedEntity.Functions.Where(e => e.Generate)) {
//				this.FunctionGenerator.GenerateOpaque (typedEntity, functionEntity, OPAQUE_POINTER);
				this.Writer.WriteLine ();
			}

			// Append class ender
			this.Writer.WriteLineFormat (1, "}}");

			// Append static condition if needed
			this.AppendEndCondition (typedEntity);

			// Append namespace ender
			this.Writer.WriteLineFormat (0, "}}");
		}
	}
}